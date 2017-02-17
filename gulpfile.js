var gulp = require("gulp"),
	concat = require("gulp-concat"),
	rename = require("gulp-rename"),
	cssmin = require("gulp-cssmin"),
	less = require("gulp-less"),
	uglify = require("gulp-uglify")

var srcDir = 'wwwroot/assets/'
var cssTargetDir = 'wwwroot/css/'
var jsTargetDir = 'wwwroot/js/'

gulp.task('notes-css', function() {
	return gulp.src([
		srcDir + 'css/notes.less'
	])
	.pipe(less())
	.pipe(concat('notes.min.css'))
	.pipe(cssmin())
	.pipe(gulp.dest(cssTargetDir));
});

gulp.task('admin2-js', function() {
	return gulp.src([
		srcDir + 'js/sb-admin-2.js'
	])
	.pipe(uglify())
	.pipe(rename({suffix: '.min'}))
	.pipe(gulp.dest(jsTargetDir))
});

gulp.task('notes-js', function() {
	return gulp.src([
		srcDir + 'js/notes.js'
	])
	.pipe(uglify())
	.pipe(rename({suffix: '.min'}))
	.pipe(gulp.dest(jsTargetDir))
});

gulp.task('default', [
	'notes-css',
	'admin2-js',
	'notes-js'
]);