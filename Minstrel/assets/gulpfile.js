const gulp = require('gulp');

const { buildIcons } = require('./gulp.tasks/arrange-icons');
const parseStations = require('./gulp.tasks/parse-stations');

exports['build-icons'] = buildIcons;
exports['parse-muzofon'] = parseStations.parseMuzofon;