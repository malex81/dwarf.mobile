const gulp = require('gulp');
const download = require("gulp-download-stream");		//https://github.com/michalc/gulp-download-stream
// const cheerio = require('gulp-cheerio');				//https://www.npmjs.com/package/gulp-cheerio
const cheerio = require('cheerio');
const through = require('through2');
const buffer = require('vinyl-buffer');

function parseMuzofon() {
	return download({
		file: "radio_stations.html",
		url: "https://muzofond.fm/radio-online"
	})
		.pipe(buffer())
		.pipe(through.obj(function (file, enc, cb) {
			// 1. Загружаем содержимое в cheerio вручную
			const $ = cheerio.load(file.contents.toString());

			const tracks = [];
			$('.item').each((i, el) => {
				tracks.push({
					title: $(el).find('.title').text().trim()
				});
			});

			// 2. Вот теперь ПЕРЕЗАПИСЫВАЕМ содержимое на JSON
			file.contents = Buffer.from(JSON.stringify(tracks, null, 2));

			// Передаем измененный файл дальше
			cb(null, file);
		}))
		.pipe(gulp.dest("./temp/"));
}

module.exports = {
	parseMuzofon
}