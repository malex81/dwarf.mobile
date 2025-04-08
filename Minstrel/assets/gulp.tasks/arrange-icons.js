// https://github.com/sindresorhus/gulp-template - вместо consolidate

const gulp = require('gulp');
const rename = require('gulp-rename');
const svgMin = require('gulp-svgmin');

function buildIcons() {
	return gulp.src(['./svg/**/*.svg'])
		.pipe(svgMin())
		.pipe(rename(function (path) {
			path.dirname = path.dirname + "/temp";
		}))
		.pipe(gulp.dest('.'));
}

module.exports = {
	buildIcons
}