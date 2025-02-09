using Microsoft.EntityFrameworkCore;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddCommandLine(["dev"]);
builder.Services.AddCors();
string Home = builder.Environment.ContentRootPath;
builder.Services.AddDbContext<Context>(opt => {
	opt.UseSqlite($"Data Source={Home}{Path.DirectorySeparatorChar}Database.sqlite");
});
var app = builder.Build();
using (var scope = app.Services.CreateScope())
	scope.ServiceProvider.GetRequiredService<Context>().Database.EnsureCreated();
app.UseCors(p => {
	p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
});

QuestPDF.Settings.License = LicenseType.Community;
FontManager.RegisterFont(File.OpenRead($"{Home}/Fonts/BNazanin.ttf"));
FontManager.RegisterFont(File.OpenRead($"{Home}/Fonts/KingFahdBasmalah.otf"));
FontManager.RegisterFont(File.OpenRead($"{Home}/Fonts/IranNastaliq.ttf"));

app.MapGet("/isAlive", (IConfiguration config) => {
	if ((config["dev"]?.Length ?? 0) != 0)
		return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
	if (config["dev"] != null)
		return Results.Text(config["dev"]);
	return Results.Text("0");
});
app.MapGet("json", (Context db) => {
	return Results.Json(db.Articles);
});
app.MapGet("/certificate", (Context db) => {
	var doc = Document.Create(c => {
		foreach (var article in db.Articles)
			if (article.State == State.Accepted)
				for (int i = 0; i < article.Authors.Length; i++)
					if (article.Authors[i] != "احمد رضا محرابیان")
						c.Page(p => p.CertificatePage(article, i));
	});
	return Results.File(doc.GeneratePdf(), "application/pdf", "Certificates.pdf");
});
app.MapPost("/new", async (Article article, Context db) => {
	db.Add(article);
	await db.SaveChangesAsync();
	var doc = Document.Create(c => {
		c.Page(p => {
			p.Background().AlignMiddle().AlignCenter().Text(t => {
				t.DefaultTextStyle(QuestExtensions.BaseStyle.Bold().FontColor(Color.FromRGB(0xD0, 0xD0, 0xD0)));
				t.Line("محرمانه").FontSize(36);
				t.Line("حاوی شماره\u200Cهای تماس شرکت\u200Cکنندگان").FontSize(30);
			});
			p.ArticlePage(article, $"{article.Telephone} - ID: {article.ArticleID}");
		});
	});
	return Results.File(doc.GeneratePdf(), "application/pdf", $"{article.ArticleID}-{article.State}.pdf");
});
app.MapGet("/get/{id}", async (int id, Context db) => {
	Article? article = await db.Articles.FindAsync(id);
	if (article == null)
		return Results.NotFound();
	return Results.Json(article);
});
app.MapGet("/pending", (Context db) => {
	return Results.Text(db.Articles
		.AsParallel()
		.Where(article => article.State == State.Pending)
		.Select(article => article.ArticleID)
		.Aggregate("", (acc, e) => acc + e + ", ", acc => acc.TrimEnd().TrimEnd(',')));
});
#if DEBUG
app.MapGet("/booklet", (Context db) => {
#else
app.MapGet("/booklet/{password}", (string password, Context db, IConfiguration config) => {
	if (password != config["Password"])
		return Results.StatusCode(StatusCodes.Status403Forbidden);
#endif
	Article[] ConferenceArticles = db.Articles
		.Where(art => art.State == State.Conference)
		.AsEnumerable()
		.OrderBy(art => art.Title, StringComparer.Create(new CultureInfo("fa-IR"), true))
		.ToArray();
	Article[] PosterArticles = db.Articles
		.Where(art => art.State == State.Accepted)
		.AsEnumerable()
		.OrderBy(art => art.Title, StringComparer.Create(new CultureInfo("fa-IR"), true))
		.ToArray();
	string[] WriterProfile = ConferenceArticles
				.SelectMany((article, i) => article.Authors.Select(author => ValueTuple.Create(author, i + 2)))
				.Concat(
					PosterArticles.SelectMany((article, i) => article.Authors.Select(author => ValueTuple.Create(author, i + ConferenceArticles.Length + 3)))
				)
				.GroupBy(tu => tu.Item1, tu => tu.Item2, StringComparer.Create(new CultureInfo("fa-IR"), true))
				.OrderBy(g => g.Key)
				.Select(g =>
					g.Aggregate($"{g.Key}: ", (acc, i) => acc + i + "، ", acc => acc.TrimEnd().TrimEnd('،'))
				)
				.ToArray();
	string[] KeywordProfile = ConferenceArticles
				.SelectMany((article, i) => article.Keywords.Select(keyword => ValueTuple.Create(keyword, i + 2)))
				.Concat(
					PosterArticles.SelectMany((article, i) => article.Keywords.Select(keyword => ValueTuple.Create(keyword.Trim(), i + ConferenceArticles.Length + 3)))
				)
				.GroupBy(tu => tu.Item1, tu => tu.Item2, StringComparer.Create(new CultureInfo("fa-IR"), true))
				.OrderBy(g => g.Key)
				.Select(g =>
					g.Aggregate($"{g.Key}: ", (acc, i) => acc + i + "، ", acc => acc.TrimEnd().TrimEnd('،'))
				)
				.ToArray();
	var doc = Document.Create(c => {
		c.Booklet(ConferenceArticles, PosterArticles, WriterProfile, KeywordProfile);
	});
	var meta = doc.GetMetadata();
	meta.Author = "AlLiberali";
	meta.Subject = "Bee product symposium article abstract collection booklet";
	meta.Title = "Symposium Articles' Booklet";
	meta.Creator = "Faculty of life sciences and technology, Shahid Beheshti University";
	meta.Producer = "QuestPDF .NET";
	return Results.File(doc.GeneratePdf(), "application/pdf", "Booklet.pdf");
});
#if DEBUG
app.MapPut("/update", async (Article update, Context db) => {
#else
app.MapPut("/update/{password}", async (string password, Article update, Context db, IConfiguration config) => {
	if (password != config["Password"])
		return Results.StatusCode(StatusCodes.Status403Forbidden);
#endif
	db.Update(update);
	await db.SaveChangesAsync();
	return Results.Ok();
});
#if DEBUG
app.MapGet("/preview/{id}", async (int id, Context db) => {
#else
app.MapGet("/preview/{password}/{id}", async (string password, int id, Context db, IConfiguration config) => {
	if (password != config["Password"])
		return Results.StatusCode(StatusCodes.Status403Forbidden);
#endif
	Article? article = await db.Articles.FindAsync(id);
	if (article == null)
		return Results.NotFound();
	var doc = Document.Create(c => {
		c.Page(p => {
			p.Background().AlignMiddle().AlignCenter().Text(t => {
				t.DefaultTextStyle(QuestExtensions.BaseStyle.Bold().FontColor(Color.FromRGB(0xD0, 0xD0, 0xD0)));
				t.Line("محرمانه").FontSize(36);
				t.Line("حاوی شماره\u200Cهای تماس شرکت\u200Cکنندگان").FontSize(30);
			});
			p.ArticlePage(article, $"{article.Telephone} - ID: {article.ArticleID}");
		});
	});
	return Results.File(doc.GeneratePdf(), "application/pdf", $"{article.ArticleID}-{article.State}.pdf");
});
app.Run();

public class Article {
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int ArticleID { get; set; }
	public State State { get; set; }
	public int Ordering { get; set; }
	public string? Notes { get; set; }
	public required string Title { get; set; }
	public int Respondant { get; set; }
	public required string Email { get; set; }
	public required string Telephone { get; set; }
	public required string[] Authors { get; set; }
	public required string[] Affiliations { get; set; }
	public required string[] Keywords { get; set; }
	public required string Abstract { get; set; }
}
public enum State {
	Pending, Accepted, Conference
}
public class Context : DbContext {
	public Context(DbContextOptions<Context> options) : base(options) {
	}
	public DbSet<Article> Articles { get; set; }
	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		modelBuilder.Entity<Article>().ToTable("Article");
	}
}