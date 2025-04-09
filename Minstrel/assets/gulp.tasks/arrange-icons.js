// https://github.com/sindresorhus/gulp-template - вместо consolidate
// https://www.npmjs.com/package/gulp-svgmin

const gulp = require('gulp');
const rename = require('gulp-rename');
const svgMin = require('gulp-svgmin');

const pathApi = require('path');

const minstrelResDir = '../Dwarf.Minstrel/Resources';

function buildIcons() {
	return gulp.src(['./svg/**/*.svg'])
		.pipe(svgMin({
			multipass: true,
			plugins: [
				'removeDoctype',
				'removeComments',
				'cleanupIDs'
			],
		}))
		.pipe(rename(function (path) {
			let dest = path.basename.startsWith('logo') ? 'AppIcon'
				: path.basename.startsWith('splash') ? 'Splash'
					: 'Images'
			path.dirname = pathApi.join(minstrelResDir, dest);
		}))
		.pipe(gulp.dest('.'));
}

module.exports = {
	buildIcons
}