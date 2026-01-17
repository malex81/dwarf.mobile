const gulp = require('gulp');
var download = require("gulp-download-stream");

function parseMuzofon() {
	return download({
		file: "radio_stations.html",
		url: "https://muzofond.fm/"
	}).pipe(gulp.dest("./temp/"));
}

module.exports = {
	parseMuzofon
}