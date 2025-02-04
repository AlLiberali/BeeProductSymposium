const DEBUG = true;
const BASE_URL = "http://127.0.0.1:22222/";
var notes = "";
function init() {
	isAlive();
	initButtons();
}
function isAlive() {
	let tid = 0;
	const func = (i) => {
		document.body.removeChild(document.body.firstElementChild);
		switch (i) {
			case 0:
				clearTimeout(tid);
				document.getElementById("form").style.display = "flex";
				break;
			case 99999:
				document.getElementById("warning").style.display = "flex";
				break;
			default:
				document.getElementById("warning").style.display = "flex";
				document.getElementById("warning").innerText = `در حال سرویس. ${i} دقیقه دیگر مراجعه کنید.`;
		}
	};
	tid = setTimeout(func, 20000, 99999);
	if (DEBUG)
		func(0);
	else
		fetch(BASE_URL + "isAlive")
			.then(response => response.status === 200 ? response.text() : "99999", null)
			.then(t => parseInt(t), null)
			.then(func, null);
	
}
function initButtons() {
	if (document.getElementById("authorSet").children.length == 1)
		document.getElementById("removeAuthor").setAttribute("disabled", "");
	if (document.getElementById("keywordSet").children.length == 1)
		document.getElementById("removeKeyword").setAttribute("disabled", "");
}
function addAuthorListener() {
	let set = document.getElementById("authorSet");
	let clone = set.lastElementChild.cloneNode(true);
	document.getElementById("removeAuthor").removeAttribute("disabled");
	let index = parseInt(clone.lastElementChild.lastElementChild.value);
	clone.lastElementChild.lastElementChild.checked = false;
	let inserted = set.appendChild(clone);
	inserted.removeAttribute("id");
	index++;
	inserted.lastElementChild.lastElementChild.value = index;
	inserted.firstElementChild.innerText = index.toString().replaceAll('1', '\u06F1')
		.replaceAll('2', '\u06F2')
		.replaceAll('3', '\u06F3')
		.replaceAll('4', '\u06F4')
		.replaceAll('5', '\u06F5')
		.replaceAll('6', '\u06F6')
		.replaceAll('7', '\u06F7')
		.replaceAll('8', '\u06F8')
		.replaceAll('9', '\u06F9')
		.replaceAll('0', '\u06F0');
	inserted.children[1].children[0].value = "";
	inserted.children[1].children[1].value = "";
}
function removeAuthorListener() {
	let set = document.getElementById("authorSet");
	if (set.children.length == 2) {
		document.getElementById("removeAuthor").setAttribute("disabled", "");
	}
	set.removeChild(set.lastElementChild);
}
function addKeywordListener() {
	let set = document.getElementById("keywordSet");
	let clone = document.getElementById("keywordInit").cloneNode(true);
	document.getElementById("removeKeyword").removeAttribute("disabled");
	let inserted = set.appendChild(clone);
	inserted.removeAttribute("id");
	inserted.value = "";
}
function removeKeywordListener() {
	let set = document.getElementById("keywordSet");
	if (set.children.length == 2) {
		document.getElementById("removeKeyword").setAttribute("disabled", "");
	}
	set.removeChild(set.lastElementChild);
}
function encapsulation() {
	let title = document.getElementById("title").value;
	let abstract = document.getElementById("abstract").value;
	let email = document.getElementById("email").value;
	let telephone = document.getElementById("telephone").value;
	let keywords = document.getElementsByClassName("keyword");
	let affiliations = document.getElementsByClassName("affiliation");
	let names = document.getElementsByClassName("name");
	let respondant = parseInt(document.querySelector("input[name=respondant]:checked")?.value);
	let empty = title === "" || abstract === "" || email === "" || telephone === "" || isNaN(respondant);
	let authorNames = [];
	for (let i = 0; i < names.length; i++) {
		if (names[i].value === "") {
			empty = true;
			break;
		}
		authorNames.push(names[i].value);
	}
	let authorAffiliations = [];
	for (let i = 0; i < affiliations.length; i++) {
		if (affiliations[i].value === "") {
			empty = true;
			break;
		}
		authorAffiliations.push(affiliations[i].value);
	}
	let keywordArray = [];
	for (let i = 0; i < keywords.length; i++) {
		if (keywords[i].value === "") {
			empty = true;
			break;
		}
		keywordArray.push(keywords[i].value);
	}
	if (empty) {
		alert("همه موارد الزامی است. لطفا فرم را تکمیل کنید.\nدر صورت اضافی بودن ورودی برای اسامی نویسندگان یا کلمات کلیدی می\u200Cتوانید از دکمه منفی مربوطه استفاده کنید.");
		return;
	}
	return {
		Title: title,
		Respondant: respondant - 1,
		Authors: authorNames,
		Affiliations: authorAffiliations,
		Email: email,
		Telephone: telephone,
		Abstract: abstract,
		Keywords: keywordArray
	};
}

function send() {
	fetch(BASE_URL + "new", {
		method: "POST",
		body: JSON.stringify(encapsulation()),
		headers: {
			"Content-Type": "application/json"
		}
	})
		.then(res => res.blob())
		.then(blob => URL.createObjectURL(blob))
		.then(sent);
}

function sent(bloburl) {
	let form = document.getElementById("form");
	form.removeChild(form.lastElementChild);
	let p = document.createElement("p");
	p.innerText = "مقاله شما دریافت شد!";
	form.appendChild(p);
	let a = document.createElement("a");
	a.innerText = "جهت دانلود پیشنمایش کتابچه کلیک کنید";
	a.href = bloburl;
	a.target = "_blank";
	a.rel = "noopener noreferrer";
	form.appendChild(a);
}

function edit(id) {
	fetch(BASE_URL + "get/" + id.toString())
		.then(res => res.json())
		.then(loadForm);
}
function loadForm(obj) {
	notes = obj.notes;
	let form = document.getElementById("form");
	form.removeChild(form.lastElementChild);
	form.children[0].innerText = obj.articleID.toString();
	document.getElementById("title").value = obj.title;
	document.getElementById("abstract").value = obj.abstract;
	document.getElementById("email").value = obj.email;
	document.getElementById("telephone").value = obj.telephone;
	let ks = document.getElementById("keywordSet");
	while (ks.childElementCount > 1)
		removeKeywordListener();
	for (i = 1; i < obj.keywords.length; i++)
		addKeywordListener();
	for (i = 0; i < ks.childElementCount; i++)
		ks.children[i].value = obj.keywords[i];
	let as = document.getElementById("authorSet");
	while (as.childElementCount > 1)
		removeAuthorListener();
	for (i = 1; i < obj.authors.length; i++)
		addAuthorListener();
	let names = document.getElementsByClassName("name");
	for (i = 0; i < names.length; i++)
		names[i].value = obj.authors[i];
	let affiliations = document.getElementsByClassName("affiliation");
	for (i = 0; i < affiliations.length; i++)
		affiliations[i].value = obj.affiliations[i];
	document.getElementsByName("respondant")[obj.respondant].checked = true;
}

function pending(password) {
	let obj = encapsulation();
	obj.state = 0;
	obj.notes = notes;
	obj.articleID = parseInt(document.getElementById("form").children[0].innerText);
	fetch(BASE_URL + "update" + (DEBUG ? "" : `/${password}`), {
		method: "PUT",
		body: JSON.stringify(obj),
		headers: {
			"Content-Type": "application/json"
		}
	})
		.then(response => console.log(response));
}
function accepted(password) {
	let obj = encapsulation();
	obj.state = 1;
	obj.notes = notes;
	obj.articleID = parseInt(document.getElementById("form").children[0].innerText);
	fetch(BASE_URL + "update" + (DEBUG ? "" : `/${password}`), {
		method: "PUT",
		body: JSON.stringify(obj),
		headers: {
			"Content-Type": "application/json"
		}
	})
		.then(response => console.log(response));
}
function conference(password) {
	let obj = encapsulation();
	obj.state = 2;
	obj.notes = notes;
	obj.articleID = parseInt(document.getElementById("form").children[0].innerText);
	fetch(BASE_URL + "update" + (DEBUG ? "" : `/${password}`), {
		method: "PUT",
		body: JSON.stringify(obj),
		headers: {
			"Content-Type": "application/json"
		}
	})
		.then(response => console.log(response));
}
