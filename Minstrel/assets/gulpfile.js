const gulp = require('gulp');

const { buildIcons } = require('./gulp.tasks/arrange-icons');

exports['build-icons'] = buildIcons;