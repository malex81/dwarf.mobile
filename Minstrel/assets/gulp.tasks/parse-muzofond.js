const gulp = require('gulp');
const download = require("gulp-download-stream");		//https://github.com/michalc/gulp-download-stream
// const cheerio = require('gulp-cheerio');				//https://www.npmjs.com/package/gulp-cheerio
const cheerio = require('cheerio');
const through = require('through2');
const buffer = require('vinyl-buffer');
const axios = require('axios');							//https://www.npmjs.com/package/axios
const Vinyl = require('vinyl');
const path = require('path');
const del = require('del');

const muzofondUrl = 'https://muzofond.fm';
const muzofondRadioUrl = `${muzofondUrl}/radio-online`;

function cleanImages() {
  return del(['./temp/images/*']);
}

function parseMuzofond() {
	return download({
		file: "radio_stations.json",
		url: muzofondUrl
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

async function parseMuzofond2() {
	let resTracks = [];
	let page = 1;
	let hasData = true;

	console.log('Начинаю сбор данных...');
	// Создаем поток Gulp из собранных данных
	const stream = through.obj();

	while (hasData) {
		try {
			const pageUrl = `${muzofondRadioUrl}/${page}`;
			console.log(`Загружаю страницу ${page}: ${pageUrl}`);

			const { data: html } = await axios.get(pageUrl, {
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

			const itemPromises = items.toArray().map(async (el) => {
				const $el = $(el);

				const id = $el.attr('data-this-radio_id');
				const imgUrl = $el.find('img').attr('data-src');
				const absImageUrl = `${muzofondUrl}${imgUrl}`;
				const dataUrl = $el.attr('data-href');
				
				let imgName = null;
				if (imgUrl) {
					try {
						const imgResponse = await axios.get(absImageUrl, { responseType: 'arraybuffer' });
						// imgName = `img_${Date.now()}_${Math.random().toString(36).slice(-5)}${path.extname(imgUrl)}`;
						imgName = `img_${id}${path.extname(imgUrl)}`;

						stream.push(new Vinyl({
							path: `images/${imgName}`,
							contents: Buffer.from(imgResponse.data)
						}));
					} catch (err) {
						console.log(`Не удалось загрузить изображение, по адресу ${absImageUrl}`);
					}
				}

				return {
					id,
					title: $el.find('.title').text().trim(),
					streamUrl: dataUrl,
					image: imgName
				};
			});

			// Ждем, пока все картинки на ТЕКУЩЕЙ странице скачаются
			const pageTracks = await Promise.all(itemPromises);
			resTracks.push(...pageTracks);

			page++;
		} catch (error) {
			console.log(`Загрузка прервана (страница ${page} не найдена или ошибка сети)`);
			hasData = false;
		}
	}

	console.log(`Всего нашли ${resTracks.length} радиостанций`);

	// Генерируем виртуальный файл из нашего массива
	const resultFile = new Vinyl({
		path: 'radio_stations.json',
		contents: Buffer.from(JSON.stringify(resTracks, null, 2))
	});

	stream.push(resultFile);
	stream.push(null); // Закрываем поток

	return stream.pipe(gulp.dest('./temp/'));
}

module.exports = gulp.series(cleanImages, parseMuzofond2);