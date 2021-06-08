module.exports = function (grunt) {
	// srcDir:       'wwwroot/assets/'
	// cssTargetDir: 'wwwroot/css/'
	// jsTargetDir:  'wwwroot/js/'

	grunt.initConfig({
		less: {
			target: {
				options: {
					paths: ["wwwroot/assets/"]
				},
				files: {
					"wwwroot/css/notes.grunt.css": "wwwroot/assets/css/notes.less",
				}
			}
		},
		cssmin: {
			target: {
				files: {
					"wwwroot/css/notes.min.css": [
						"wwwroot/css/notes.grunt.css"
					]
				}
			}
		},
		uglify: {
			target: {
				files: {
					"wwwroot/js/notes.min.js": ["wwwroot/assets/js/notes.js"]
				}
			}
		},
		clean: {
			cleanup: ["wwwroot/css/*.grunt.*"]
		}
	});

	grunt.loadNpmTasks("grunt-contrib-less");
	grunt.loadNpmTasks("grunt-contrib-cssmin");
	grunt.loadNpmTasks("grunt-contrib-uglify");
	grunt.loadNpmTasks("grunt-contrib-clean");
	grunt.registerTask("default", [
		"less",
		"cssmin",
		"uglify",
		"clean"
	]);
};