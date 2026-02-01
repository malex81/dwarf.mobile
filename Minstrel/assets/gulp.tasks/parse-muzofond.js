const gulp = require('gulp');
const download = require("gulp-download-stream");		//https://github.com/michalc/gulp-download-stream
// const cheerio = require('gulp-cheerio');				//https://www.npmjs.com/package/gulp-cheerio
const cheerio = require('cheerio');
const through = require('through2');
const buffer = require('vinyl-buffer');
const axios = require('axios');							//https://www.npmjs.com/package/axios
const Vinyl = require('vinyl');

function parseMuzofon() {
	return download({
		file: "radio_stations.json",
		url: "https://muzofond.fm/radio-online/"
	})
		.pipe(buffer())
		.pipe(through.obj(function (file, enc, cb) {
			// 1. Загружаем содержимое в cheerio вручную
			const $ = cheerio.load(file.contents.toString());

			const tracks = [];
			$('li.this_radio').each((i, el) => {
				tracks.push({
					title: $(el).find('.title').text().trim()
				});
			});

			// 2. Вот теперь ПЕРЕЗАПИСЫВАЕМ содержимое на JSON
			file.contents = Buffer.from(JSON.stringify(tracks, null, 2));

			// Передаем измененный файл дальше
			cb(null, file);

			// ---
			console.log(`${tracks.length} stations loaded`);
		}))
		.pipe(gulp.dest("./temp/"));
}

async function parseMuzofon2() {
	let resTracks = [];
	let page = 1;
	let hasData = true;

	console.log('Начинаю сбор данных...');

	while (hasData) {
		try {
			const url = `https://muzofond.fm/radio-online/${page}`;
			console.log(`Загружаю страницу ${page}: ${url}`);

			const { data: html } = await axios.get(url, {
				timeout: 10000,
				headers: { 'User-Agent': 'Mozilla/5.0' }
			});

			const $ = cheerio.load(html);
			const items = $('li.this_radio');

			// Если на странице нет элементов с классом .item — данных больше нет
			if (items.length === 0 || page > 20) { // page > 5 добавлено для теста, чтобы не уйти в бесконечный цикл
				hasData = false;
				break;
			}

			items.each((i, el) => {
				resTracks.push({
					page: page,
					title: $(el).find('.title').text().trim()
				});
			});

			page++;
		} catch (error) {
			console.log(`Загрузка прервана (страница ${page} не найдена или ошибка сети)`);
			hasData = false;
		}
	}
	
	console.log(`Всего нашли ${resTracks.length} радиостанций`);
	
	// Создаем поток Gulp из собранных данных
	const stream = through.obj();

	// Генерируем виртуальный файл из нашего массива
	const resultFile = new Vinyl({
		path: 'radio_stations.json',
		contents: Buffer.from(JSON.stringify(resTracks, null, 2))
	});

	stream.push(resultFile);
	stream.push(null); // Закрываем поток

	return stream.pipe(gulp.dest('./temp/'));
}

module.exports = parseMuzofon2