"use strict";
const gulp = require('gulp');
const sass = require('gulp-sass')(require('node-sass'));

const input = 'src/sass/**/*.scss';
const output = 'dist/css/';

let sassOptions;

sassOptions = {
  errLogToConsole: true,
    includePaths: [
    'src/govuk_template/assets/stylesheets',
    'src/govuk_frontend_toolkit/stylesheets'
  ],
};

gulp.task('watch', () => {
  return gulp.watch(input, gulp.series('sass'))
    .on('change', (event) => {
      console.log(`File ${event} was changed, running tasks...`);
    });
});

gulp.task('sass', () => {
    return gulp
        .src(input)
        .pipe(sass(sassOptions))
        .pipe(gulp.dest(output))
});

gulp.task('default', gulp.series('sass', 'watch'));