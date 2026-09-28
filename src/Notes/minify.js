import { minify } from '@node-minify/core';
import { terser } from '@node-minify/terser';
import less from 'less';

const lessBuilder = async ({ settings, content }) => {
	var result = (await less.render(content, settings.options)).css

	return { code: result };
};

minify({
	compressor: lessBuilder,
	input: 'wwwroot/assets/css/*.less',
	output: 'wwwroot/css/notes.min.css',
	options: {
		compress: true
	},
}).then((min) => {
	console.log("Minify LESS => notes.min.css.");
});


minify({
	compressor: terser,
	input: 'wwwroot/assets/js/*.js',
	output: 'wwwroot/js/notes.min.js'
}).then((min) => {
	console.log("Minify JS => notes.min.js.");
});
