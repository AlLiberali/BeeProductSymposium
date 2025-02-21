using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;

public static class QuestExtensions {
#if DEBUG
	public static readonly string Home = @"V:\workspace\Symposium\Environment";
#else
	public static readonly string Home = "/home/nader/workspace/Symposium/Environment";
#endif
	public static readonly TextStyle BaseStyle = TextStyle.Default.DirectionFromRightToLeft().FontFamily("B Nazanin");
	public static readonly TextStyle CertStyle = TextStyle.Default.DirectionFromRightToLeft().FontFamily("IranNastaliq");
	public static class Images {
		public static readonly Image SBULogo = Image.FromFile($"{Home}/Images/sbu.png");
		public static readonly Image FrontCover = Image.FromFile($"{Home}/Images/FrontCover.png");
		public static readonly Image BackCover = Image.FromFile($"{Home}/Images/BackCover.png");
		public static readonly Image SBUPark = Image.FromFile($"{Home}/Images/sbu-park.png");
		public static readonly Image SBUVP = Image.FromFile($"{Home}/Images/sbu-respvp.png");
		public static readonly Image HBRCLogo = Image.FromFile($"{Home}/Images/HBRC-SBU.png");
		public static readonly Image MedPlantAssoc = Image.FromFile($"{Home}/Images/MedPlantAssoc.png");
		public static readonly Image NutriAssoc = Image.FromFile($"{Home}/Images/NutriAssoc.png");
		public static readonly Image ShamsLogo = Image.FromFile($"{Home}/Images/Shams.png");
		public static readonly Image PhlomisLogo = Image.FromFile($"{Home}/Images/Phlomis.png");
		public static readonly Image PhloPharmaLogo = Image.FromFile($"{Home}/Images/PhlomisPharma.png");
		public static readonly Image ConferenceLogo = Image.FromFile($"{Home}/Images/ConferenceLogo.png");
		public static readonly Image Golbarg = Image.FromFile($"{Home}/Images/Golbarg.png");
		public static readonly Image[] Logos = Directory.EnumerateFiles($"{Home}/Images/Logo").AsParallel().Select(Image.FromFile).ToArray();
	}
	public static class Texts {
		public static readonly string Hamidi = File.ReadAllText($"{Home}/Text/AqaMiri.txt");
		public static readonly string Razzaz = File.ReadAllText($"{Home}/Text/Razzaz.txt");
		public static readonly string Mehrabian = File.ReadAllText($"{Home}/Text/Mehrabian.txt");
		public static readonly string[] Introduction = File.ReadAllLines($"{Home}/Text/Introduction.txt");
		public static readonly string[] Schedule = File.ReadAllLines($"{Home}/Text/Schedule.txt");
		public static readonly string[] ScientificCommittee = File.ReadAllLines($"{Home}/Text/ScientificCommittee.txt");
		public static readonly string[] ExecutiveCommittee = File.ReadAllLines($"{Home}/Text/ExecutiveCommittee.txt");
		public static readonly string[] LeadershipCommittee = File.ReadAllLines($"{Home}/Text/LeadershipCommittee.txt");
		public static readonly string[] ResearchCentreIntro = Directory.EnumerateFiles($"{Home}/Text/ResearchCentre")
			.Select(File.ReadAllText)
			.ToArray();
	}
	public static IDocumentContainer Booklet(this IDocumentContainer container, Article[] ConferenceArticles, Article[] PosterArticles, string[] WriterProfile, string[] KeywordProfile) {
		container.Page(p => {
			p.Size(PageSizes.A4);
			p.Content().Width(595).Height(842).Image(Images.FrontCover).WithCompressionQuality(ImageCompressionQuality.Best);
		});
		container.Page(p => {
			p.Size(PageSizes.A4);
			p.ContentFromRightToLeft();
			p.Content().AlignCenter().Text("N").FontFamily("KFGQPC Basmalah").FontSize(140);
		});
		container.Page(p => {
			p.Size(PageSizes.A4);
			p.ContentFromRightToLeft();
			p.Content().AlignCenter().AlignMiddle().Text(t => {
				t.DefaultTextStyle(BaseStyle);
				t.Element().Height(4, Unit.Centimetre).Image(Images.ConferenceLogo);
				t.EmptyLine();
				t.Line("کتابچه چکیده مقالات").FontSize(22);
				t.Line("اولین سمپوزیوم ملی فرآورده\u200Cهای\n غذایی و دارویی زنبور عسل").FontSize(36).Bold();
				t.EmptyLine();
				t.EmptyLine();
				t.EmptyLine();
				t.EmptyLine();
				t.EmptyLine();
				t.EmptyLine();
				t.Line("17 بهمن 1403");
			});
		});
		container.Page(p => {
			p.Size(PageSizes.A4);
			p.Content().AlignCenter().AlignMiddle().Image($"{Home}/Images/Poster.png").FitArea();
		});
		container.Page(p => {
			p.Size(PageSizes.A4);
			p.ContentFromRightToLeft();
			p.Margin(2, Unit.Millimetre);
			p.StandardHeader()
			.StandardFooter("حامیان")
			.Content()
			.PaddingHorizontal(22, Unit.Millimetre)
			.PaddingTop(9, Unit.Millimetre)
			.AlignCenter()
			.Table(t => {
				t.ColumnsDefinition(tcd => {
					tcd.RelativeColumn();
					tcd.RelativeColumn();
					tcd.RelativeColumn();
					tcd.RelativeColumn();
				});
				foreach (var logo in Images.Logos) {
					t.Cell()
					.Height(4, Unit.Centimetre)
					.Width(4, Unit.Centimetre)
					.AlignCenter()
					.AlignMiddle()
					.Image(logo).FitArea();
				}
			});
		});
		container.Page(p => {
			p.Size(PageSizes.A4);
			p.Content().Width(595).Height(842).Image(Images.Golbarg).WithCompressionQuality(ImageCompressionQuality.Best);
		});
		container.Page(p => {
			p.Size(PageSizes.A4);
			p.ContentFromRightToLeft();
			p.Margin(2, Unit.Millimetre);
			p.StandardHeader()
			.StandardFooter("معرفی مرکز")
			.Content()
			.PaddingHorizontal(0.7f, Unit.Centimetre)
			.PaddingVertical(0.5f, Unit.Centimetre)
			.Column(col => {
				col.Item().BorderBottom(1).Text("معرفی مرکز پژوهشی فرآورده\u200Cهای زنبور عسل دانشگاه شهید بهشتی").Style(BaseStyle.Bold().FontSize(16));
				col.Item().Text(Texts.ResearchCentreIntro[0]).Justify().Style(BaseStyle);
				col.Item().Text("    بیانیۀ چشم\u200Cانداز").Style(BaseStyle.Bold());
				col.Item().Text(Texts.ResearchCentreIntro[1]).Justify().Style(BaseStyle);
				col.Item().Text("    مأموریت").Style(BaseStyle.Bold());
				col.Item().Text(Texts.ResearchCentreIntro[2]).Justify().Style(BaseStyle);
				col.Item().Text("    اهداف").Style(BaseStyle.Bold());
				col.Item().Text(Texts.ResearchCentreIntro[3]).Justify().Style(BaseStyle);
				col.Item().Text("    شیوه اجرائی، پژوهشی و تجاری\u200Cسازی").Style(BaseStyle.Bold());
				col.Item().Text(Texts.ResearchCentreIntro[4]).Justify().Style(BaseStyle);
				col.Item().Text("        بخش\u200Cهای پژوهشی در قالب همکاری\u200Cهای ملی و بین\u200Cالمللی").Style(BaseStyle.ExtraBold());
				col.Item().Text("الف) مدیریت تولید فرآورده\u200Cهای زنبور عسل").Style(BaseStyle.Bold());
				col.Item().Text(Texts.ResearchCentreIntro[5]).Style(BaseStyle);
				col.Item().Text("ب) ارزیابی، اصالت سنجی، کنترل کیفیت و رتبه\u200Cبندی فرآورده\u200Cهای زنبور عسل").Style(BaseStyle.Bold());
				col.Item().Text(Texts.ResearchCentreIntro[6]).Style(BaseStyle);
				col.Item().Text("ج) تجاری\u200Cسازی").Style(BaseStyle.Bold());
				col.Item().Text(Texts.ResearchCentreIntro[7]).Style(BaseStyle);
				col.Item().Text("    دستاوردهای کلیدی مرکز").Style(BaseStyle.Bold());
				col.Item().Text(Texts.ResearchCentreIntro[8]).Justify().Style(BaseStyle);
				col.Item().Text("    متخصصین و حامیان مرکز").Style(BaseStyle.Bold());
				col.Item().Text(Texts.ResearchCentreIntro[9]).Justify().Style(BaseStyle);
				col.Item().Text("");
				col.Item().Text(Texts.ResearchCentreIntro[10]).Style(BaseStyle);
				col.Item().AlignLeft().Text("Email: sbu.beeproducts@gmail.com").Style(BaseStyle);
				col.Item().AlignLeft().Hyperlink("https://fbs.sbu.ac.ir/fa/zanboreasal").Text(t => {
					t.DefaultTextStyle(BaseStyle);
					t.Span("Website: ");
					t.Span("https://fbs.sbu.ac.ir/fa/zanboreasal").FontColor(Colors.Blue.Medium).Underline();
				});
			});
		});
		container.Page(p => {
			p.Size(PageSizes.A4);
			p.ContentFromRightToLeft();
			p.Margin(2, Unit.Millimetre);
			StandardHeader(p);
			StandardFooter(p, "معرفی سمپوزیوم");
			p.Content()
			.PaddingHorizontal(0.7f, Unit.Centimetre)
			.PaddingVertical(0.5f, Unit.Centimetre)
			.Column(col => {
				col.Item().BorderBottom(1).Text("سمپوزیوم ملی فرآورده‌های غذایی و دارویی زنبور عسل").Style(BaseStyle.Bold().FontSize(16));
				foreach (var line in Texts.Introduction) {
					switch (line[0]) {
						case '&':
							col.Item().Text(line.Substring(1)).Style(BaseStyle.ExtraBold());
							break;
						case '%':
							col.Item().Text(line.Substring(1)).Style(BaseStyle.Bold());
							break;
						default:
							col.Item().Text(line).Style(BaseStyle);
							break;
					}
				}
			});
		});
		container.Page(p => {
			CommitteePage(p, Texts.LeadershipCommittee, "شورا مدیریت سمپوزیوم");
		});
		container.Page(p => {
			CommitteePage(p, Texts.ScientificCommittee, "اعضاء کمیته علمی");
		});
		container.Page(p => {
			CommitteePage(p, Texts.ExecutiveCommittee, "اعضاء کمیته اجرائی");
			p.Foreground()
				.AlignBottom()
				.AlignLeft()
				.Text($"Powered by Open Source Software; QuestPDF .NET\nhttps://github.com/AlLiberali")
				.FontFamily("Times New Roman")
				.FontSize(8)
				.FontColor(Colors.Grey.Lighten1);
		});
		container.Page(p => {
			p.Size(PageSizes.A4);
			p.ContentFromRightToLeft();
			p.Margin(2, Unit.Millimetre);
			StandardHeader(p);
			//			StandardFooter(p, "برنامۀ زمانی سمپوزیوم");
			p.Content()
			.PaddingHorizontal(0.7f, Unit.Centimetre)
			//			.PaddingTop(0.5f, Unit.Centimetre)
			.Table(tbl => {
				tbl.ColumnsDefinition(cd => {
					cd.RelativeColumn(0.8f);
					cd.RelativeColumn(3.2f);
					cd.RelativeColumn(1.8f);
					cd.RelativeColumn();
				});
				tbl.Header(header => {
					header.Cell()
						.ColumnSpan(4)
						.Row(1)
						.Column(1)
						.BorderBottom(4)
						.AlignCenter()
						.AlignMiddle()
						.Text("برنامۀ زمانی سمپوزیوم")
						.Style(BaseStyle.Bold().FontSize(16));
					header.Cell()
						.Row(2)
						.Column(1)
						.BorderVertical(1)
						.BorderBottom(1)
						.AlignCenter()
						.AlignMiddle()
						.Text("ساعت")
						.Style(BaseStyle.Bold().FontSize(14));
					header.Cell()
						.Row(2)
						.Column(2)
						.BorderVertical(1)
						.BorderBottom(1)
						.AlignCenter()
						.AlignMiddle()
						.Text("عنوان سخنرانی/کارگاه/میزگرد/...")
						.Style(BaseStyle.Bold().FontSize(14));
					header.Cell()
						.Row(2)
						.Column(3)
						.BorderVertical(1)
						.BorderBottom(1)
						.AlignCenter()
						.AlignMiddle()
						.Text("سخنران")
						.Style(BaseStyle.Bold().FontSize(14));
					header.Cell()
						.Row(2)
						.Column(4)
						.BorderVertical(1)
						.BorderBottom(1)
						.AlignCenter()
						.AlignMiddle()
						.Text("تخصص سخنران")
						.Style(BaseStyle.Bold().FontSize(14));
				});
				uint row = 1;
				foreach (var line in Texts.Schedule) {
					if (line[0] == '*') {
						tbl.Cell()
							.Row(row)
							.Column(1)
							.Border(1)
							.AlignCenter()
							.AlignMiddle()
							.Text(line.Split('|')[0].Substring(1))
							.Style(BaseStyle);
						tbl.Cell()
							.ColumnSpan(3)
							.Row(row)
							.Column(2)
							.Border(1)
							.AlignCenter()
							.AlignMiddle()
							.Text(line.Split('|')[1])
							.Style(BaseStyle);
					} else {
						tbl.Cell()
							.Row(row)
							.Column(1)
							.Border(1)
							.AlignCenter()
							.AlignMiddle()
							.Text(line.Split('|')[0])
							.Style(BaseStyle);
						tbl.Cell()
							.Row(row)
							.Column(2)
							.Border(1)
							.AlignCenter()
							.AlignMiddle()
							.Text(line.Split('|')[1])
							.Style(BaseStyle);
						tbl.Cell()
							.Row(row)
							.Column(3)
							.Border(1)
							.AlignCenter()
							.AlignMiddle()
							.Text(line.Split('|')[2])
							.Style(BaseStyle);
						tbl.Cell()
							.Row(row)
							.Column(4)
							.Border(1)
							.AlignCenter()
							.AlignMiddle()
							.Text(line.Split('|')[3])
							.Style(BaseStyle);
					}
					row++;
				}
			});
		});
		container.Page(p => {
			p.Size(PageSizes.A4);
			p.Margin(2, Unit.Millimetre);
			p.ContentFromRightToLeft();
			p.StandardHeader().StandardFooter("1").Content().AlignMiddle().AlignCenter().Text("مقالات پذیرفته\u200Cشده برای ارائه به صورت سخنرانی").Style(BaseStyle).FontSize(36);
		});
		foreach (var (i, article) in ConferenceArticles.Select((a, i) => (i + 2, a))) {
			container.Page(p => ArticlePage(p, article, $"{i}"));
		}
		container.Page(p => {
			p.Size(PageSizes.A4);
			p.Margin(2, Unit.Millimetre);
			p.ContentFromRightToLeft();
			p.StandardHeader().StandardFooter($"{ConferenceArticles.Length + 2}").Content().AlignMiddle().AlignCenter().Text("مقالات پذیرفته\u200Cشده برای ارائه به صورت پوستر").Style(BaseStyle).FontSize(36);
		});
		foreach (var (i, article) in PosterArticles.Select((a, i) => (i + ConferenceArticles.Length + 3, a))) {
			container.Page(p => ArticlePage(p, article, $"{i}"));
		}
		/*
		container.Page(p => {
			MessagePage(p,
				"پیام رئیس سمپوزیوم",
				"بسم خالق الحب نبدأ",
				"دکتر سیده مهری حمیدی سنگدهی",
				Texts.Hamidi
			);
		});
		*/
		container.Page(p => {
			MessagePage(p,
				"پیام دبیر علمی سمپوزیوم",
				"بنام خدا",
				"دکتر جلال\u200Cالدین میرزای رزاز\nدبیر علمی سمپوزیوم، دانشیار دانشگاه علوم پزشکی شهید بهشتی و رئیس انجمن تغذیه ایران",
				Texts.Razzaz
			);
		});
		container.Page(p => {
			MessagePage(p,
				"پیام دبیر اجرائی سمپوزیوم",
				"بسم الله قاصم الجبارین",
				"دکتر احمدرضا محرابیان\nدبیر اجرائی سمپوزیوم و مدیر مرکز پژوهشی فرآورده\u200Cهای زنبور عسل دانشگاه شهید بهشتی",
				Texts.Mehrabian
			);
		});
		container.Page(p => {
			p.Size(PageSizes.A4);
			p.ContentFromRightToLeft();
			p.Margin(2, Unit.Millimetre);
			StandardHeader(p);
			StandardFooter(p, "فهرست");
			p.Content()
				.PaddingHorizontal(1, Unit.Centimetre)
				.PaddingTop(0.5f, Unit.Centimetre)
				.Table(tbl => {
					tbl.ColumnsDefinition(cd => {
						cd.RelativeColumn();
						cd.ConstantColumn(1, Unit.Centimetre);
					});
					tbl.Header(head => {
						head.Cell().Row(1).Column(1).BorderHorizontal(1).Text("عنوان مقاله").Style(BaseStyle).AlignStart().Bold().FontSize(14);
						head.Cell().Row(1).Column(2).BorderHorizontal(1).Text("صفحه").Style(BaseStyle).AlignStart().Bold().FontSize(14);
					});
					uint rIndex = 1;
					tbl.Cell().Row(rIndex).Column(1).Text("مقالات پذیرفته\u200Cشده برای ارائه به صورت سخنرانی").Style(BaseStyle).AlignStart().Bold().FontSize(10);
					tbl.Cell().Row(rIndex).Column(2).Text("\u06F1").Style(BaseStyle).AlignCenter().Bold().FontSize(10);
					foreach (var (i, title) in ConferenceArticles.Select((a, i) => (i + 1, a.Title))) {
						rIndex++;
						tbl.Cell().Row(rIndex).Column(1).Text(title).Style(BaseStyle).AlignStart().FontSize(10);
						tbl.Cell().Row(rIndex).Column(2).Text($"{i + 1}").Style(BaseStyle).AlignCenter().FontSize(10);
					}
					rIndex++;
					tbl.Cell().Row(rIndex).Column(1).Text("مقالات پذیرفته\u200Cشده برای ارائه به صورت پوستر").Style(BaseStyle).AlignStart().Bold().FontSize(10);
					tbl.Cell().Row(rIndex).Column(2).Text($"{rIndex}").Style(BaseStyle).AlignCenter().Bold().FontSize(10);
					foreach (var (i, title) in PosterArticles.Select((a, i) => (i + ConferenceArticles.Length + 3, a.Title))) {
						rIndex++;
						tbl.Cell().Row(rIndex).Column(1).Text(title).Style(BaseStyle).AlignStart().FontSize(10);
						tbl.Cell().Row(rIndex).Column(2).Text($"{i}").Style(BaseStyle).AlignCenter().FontSize(10);
					}
				});
		});
		container.Page(p => {
			p.Size(PageSizes.A4);
			p.ContentFromRightToLeft();
			StandardHeader(p);
			StandardFooter(p, "نمایۀ نویسندگان");
			p.Margin(2, Unit.Millimetre);
			p.Content()
				.PaddingHorizontal(1, Unit.Centimetre)
				.PaddingTop(0.5f, Unit.Centimetre)
				.MultiColumn(mc => {
					mc.Columns(4);
					mc.BalanceHeight(false);
					mc.Content()
				.Column(col => {
					foreach (var group in WriterProfile.GroupBy(w => w[0])) {
						col.Item().Text($"{group.Key}").Style(BaseStyle.Bold().Underline().FontSize(10));
						foreach (var writer in group)
							col.Item().Text(writer).Style(BaseStyle.FontSize(10));
					}
				});
				});
		});
		container.Page(p => {
			p.Size(PageSizes.A4);
			p.ContentFromRightToLeft();
			StandardHeader(p);
			StandardFooter(p, "نمایۀ کلمات کلیدی");
			p.Margin(2, Unit.Millimetre);
			p.Content()
				.PaddingHorizontal(1, Unit.Centimetre)
				.PaddingTop(0.5f, Unit.Centimetre)
				.MultiColumn(mc => {
					mc.Columns(4);
					mc.BalanceHeight(false);
					mc.Content()
				.Column(col => {
					foreach (var group in KeywordProfile.GroupBy(w => w[0])) {
						col.Item().Text($"{group.Key}").Style(BaseStyle.Bold().Underline().FontSize(10));
						foreach (var writer in group)
							col.Item().Text(writer).Style(BaseStyle.FontSize(10));
					}
				});
				});
		});
		container.Page(p => {
			p.Size(PageSizes.A4);
			p.Content().Width(595).Height(842).Image(Images.BackCover).WithCompressionQuality(ImageCompressionQuality.Best);
		});
		return container;
	}
	public static PageDescriptor StandardHeader(this PageDescriptor p) {
		p.Header().Height(2.7f, Unit.Centimetre).Table(t => {
			t.ColumnsDefinition(cd => {
				cd.ConstantColumn(2.7f, Unit.Centimetre);
				cd.RelativeColumn();
				cd.ConstantColumn(2.7f, Unit.Centimetre);
			});
			t.Cell().Row(1).Column(1).Image(Images.SBULogo).FitArea();
			t.Cell().Row(1).Column(3).Image(Images.HBRCLogo).FitArea();
			t.Cell().Row(1).Column(2)
			.AlignMiddle()
			.Text(t => {
				t.AlignCenter();
				t.DefaultTextStyle(BaseStyle.Bold());
				t.Line("سمپوزیوم ملی فرآورده\u200Cهای غذایی و دارویی زنبور عسل").FontSize(22);
				t.Line("\u06F1\u06F7 بهمن \u06F1\u06F4\u06F0\u06F3").FontSize(12);
			});
		});
		return p;
	}
	public static PageDescriptor StandardFooter(this PageDescriptor p, string? pageIndex = null) {
		if (pageIndex != null)
			p.Footer().Height(1f, Unit.Centimetre).AlignBottom().AlignCenter().Text(pageIndex).AlignCenter().Style(BaseStyle).FontSize(10);
		return p;
	}
	public static void CommitteePage(PageDescriptor p, string[] committee, string title) {
		p.Size(PageSizes.A4);
		p.ContentFromRightToLeft();
		p.Margin(2, Unit.Millimetre);
		StandardHeader(p);
		StandardFooter(p, title);
		p.Content()
		.PaddingHorizontal(0.7f, Unit.Centimetre)
		.PaddingTop(0.5f, Unit.Centimetre)
		.Column(col => {
			col.Item().BorderBottom(1).Text(title).Style(BaseStyle.Bold().FontSize(16));
			foreach (var member in committee) {
				col.Item().Text($"\u2022\t{member}").Style(BaseStyle);
			}
		});
	}
	public static void MessagePage(PageDescriptor p, string title, string basmalah, string name, string message) {
		p.Size(PageSizes.A4);
		p.ContentFromRightToLeft();
		p.Margin(2, Unit.Millimetre);
		StandardHeader(p);
		StandardFooter(p, title);
		p.Content()
		.PaddingHorizontal(0.7f, Unit.Centimetre)
		.PaddingTop(0.5f, Unit.Centimetre)
		.Column(col => {
			col.Item().BorderBottom(1).Text(title).Style(BaseStyle.Bold().FontSize(16));
			col.Item().Text(basmalah).AlignCenter().Style(BaseStyle.Bold());
			col.Item().Text(message).AlignStart().Justify().Style(BaseStyle);
			col.Item().Text(name).AlignCenter().Style(BaseStyle.Bold());
		});
	}

	public static void CertificatePage(this PageDescriptor page, Article article, int author) {
		page.Size(PageSizes.A4.Landscape());
		page.ContentFromRightToLeft();
		page.Margin(2, Unit.Centimetre);
		page.Header().Height(3.5f, Unit.Centimetre).Table(t => {
			t.ColumnsDefinition(cd => {
				cd.ConstantColumn(3.5f, Unit.Centimetre);
				cd.RelativeColumn();
				cd.ConstantColumn(3.5f, Unit.Centimetre);
			});
			t.Cell().Row(1).Column(1).Image(Images.SBUVP).FitArea();
			t.Cell().Row(1).Column(3).Image(Images.HBRCLogo).FitArea();
			t.Cell().Row(1).Column(2)
			.AlignMiddle()
			.AlignCenter()
			.Text(t => {
				t.AlignCenter();
				t.DefaultTextStyle(CertStyle);
				t.EmptyLine().FontSize(2);
				t.Line("گواهی ارائه مقاله در اولین سمپوزیوم فرآورده\u200Cهای غذائی و داروئی زنبور عسل").FontSize(24).Bold();
			});
		});
		page.Content()
			.AlignCenter()
			.PaddingHorizontal(3, Unit.Centimetre)
			.Text(t => {
				t.AlignCenter();
				t.DefaultTextStyle(CertStyle);
				t.Line($"بدینوسیله گواهی می\u200Cشود مقاله با عنوان").FontSize(18);
				t.Line(article.Title).FontSize(26);
				t.Span("توسط نویسنده محترم ").FontSize(18);
				t.Line(article.Authors[author]).FontSize(26);
				t.Line("در اولین سمپوزیوم فرآورده\u200Cهای غذائی و داروئی زنبور عسل دانشگاه شهید بهشتی با مجوز ISC" +
					"، کد اختصاصی \u06F7\u06F5\u06F7\u06F8\u06F3-\u06F0\u06F3\u06F2\u06F4\u06F0" +
					"در تاریخ \u06F1\u06F7 بهمن \u06F1\u06F4\u06F0\u06F3" +
					"به تائید کمیته علمی سمپوزیوم رسیده و به صورت پوستر" +
					" ارائه شده است.").FontSize(18);
			});
		page.Footer()
			.Height(3, Unit.Centimetre)
			.Table(t => {
				t.ColumnsDefinition(cd => {
					cd.RelativeColumn();
					cd.RelativeColumn();
				});
				t.Cell()
					.AlignMiddle()
					.AlignCenter()
					.Text("دبیر اجرائی سمپوزیوم و رئیس مرکز پژوهشی فرآورده\u200Cهای زنبور عسل\nاحمدرضا محرابیان")
					.Style(CertStyle)
					.FontSize(20);
				t.Cell()
					.AlignMiddle()
					.AlignCenter()
					.Text("دبیر علمی سمپوزیوم و رئیس انجمن تغذیه ایران\nجلال\u200Cالدین میرزا رزاز")
					.Style(CertStyle)
					.FontSize(20);
			});
	}
	public static void ArticlePage(this PageDescriptor page, Article article, string index) {
		page.Size(PageSizes.A4);
		page.ContentFromRightToLeft();
		page.Margin(2, Unit.Millimetre);
		page.Foreground()
			.AlignBottom()
			.AlignLeft()
			.Text($"{article.ArticleID}")
			.FontFamily("Times New Roman")
			.FontSize(8)
			.FontColor(Colors.Grey.Lighten1);
		page
			.StandardHeader()
			.StandardFooter($"{index}")
			.Content()
			.PaddingHorizontal(1, Unit.Centimetre)
			.PaddingTop(0.5f, Unit.Centimetre)
			.Column(col => {
				//			col.Spacing(0.5f, Unit.Centimetre);
				col.Item().Text(t => {
					t.AlignCenter();
					t.DefaultTextStyle(BaseStyle.Bold().FontSize(16));
					t.Span(article.Title);
				});
				col.Spacing(0.2f, Unit.Centimetre);
				col.Item().Text(t => {
					t.AlignCenter();
					t.DefaultTextStyle(BaseStyle.Bold());
					for (int i = 0; i < article.Authors.Length; i++) {
						if (i != article.Respondant) {
							t.Span(article.Authors[i]);
							t.Span($"{i + 1}").Superscript();
						} else {
							t.Span(article.Authors[i]).Underline();
							t.Span($"{i + 1}*").Superscript();
						}
						if (i + 1 != article.Authors.Length)
							t.Span("، ");
					}
					t.EmptyLine();
					for (int i = 1; i <= article.Affiliations.Length; i++) {
						t.Line($"{i}- {article.Affiliations[i - 1]}").NormalWeight().FontSize(10);
					}
					t.Span($"آدرس پست الکترونیکی نویسندۀ مسئول: {article.Email}").NormalWeight().FontSize(10);
				});
				col.Item().Text(t => {
					t.AlignStart();
					t.DefaultTextStyle(BaseStyle);
					t.Justify();
					t.Span(article.Abstract);
				});
				col.Item().Text(t => {
					t.AlignStart();
					t.DefaultTextStyle(BaseStyle);
					t.Span("کلمات کلیدی: ").Bold();
					t.Span(article.Keywords.Aggregate("", (acc, s) => acc + s.Trim() + "، ", r => r.Trim().TrimEnd('،')));
				});
			});
	}
}
