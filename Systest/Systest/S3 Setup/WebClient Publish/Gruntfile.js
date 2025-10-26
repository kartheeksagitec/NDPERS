module.exports = function (grunt) {

    // Project configuration.
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),
        clean: {
            build: ['bundles/js', 'assets.json']
        },
        concat: {
            css_and_js: {
                files: {
                    'bundles/s3-basic.min.css': ['css/bootstrap.min.css', 'css/font-awesome.min.css', 'css/toastr.min.css', 'css/jquery-ui.min.css'],
                    'bundles/s3-custom.css': ['css/layout.css', 'css/custom-bootstrap-styles.css', 'css/form-structure-styles.css', 'css/custom-jquery-ui.css',
                                            'css/loginPage.css', 'css/components.css', 'css/ExcelMatrixStyle.css', 'css/bpm-bootstrap-styles.css', 'css/style.css',
                                            'css/LogicalRuleStyle.css', 'css/Dashboard.css', 'css/EntityStyle.css', 'css/datepicker3.css', 'css/jquery.splitter.css',
                                            'css/Scenario-Styles.css', 'css/tools.css', 'css/create-new-object-styles.css', 'css/correspondence-style.css',
                                            'css/constant-style.css', 'css/file-style.css'],
                    'bundles/js/Entity/s3-Entity.js': ['Entity/scripts/**/*.js'],
                    'bundles/js/Correspondence/s3-Correspondence.js': ['Correspondence/scripts/**/*.js'],
                    'bundles/js/BPM/s3-BPM.js': ['BPM/scripts/**/*.js'],
                    'bundles/js/File/s3-File.js': ['File/scripts/**/*.js'],
                    'bundles/js/Form/s3-Form.js': ['Form/scripts/**/*.js'],
                    'bundles/js/FormLink/s3-FormLink.js': ['FormLink/scripts/**/*.js'],
                    'bundles/js/Rule/s3-Rule.js': ['Rule/scripts/**/*.js'],
                    'bundles/js/Scenario/s3-Scenario.js': ['Scenario/scripts/**/*.js'],
                    'bundles/js/CreateNewObject/s3-CreateNewObject.js': ['CreateNewObject/scripts/**/*.js'],
                    'bundles/js/Report/s3-Report.js': ['Report/scripts/**/*.js'],
                    'bundles/js/Constants/s3-Constants.js': ['Constants/scripts/**/*.js'],
                    'bundles/js/Tools/s3-Tools.js': ['Tools/scripts/**/*.js'],
                    'bundles/js/WorkFlow/s3-WorkFlow.js': ['WorkFlow/script/**/*.js'],
                    'bundles/js/StartUp/s3-StartUp.js': ['StartUp/scripts/**/*.js'],
                    'bundles/js/Common/s3-Common.js': ['Common/scripts/app.js', 'Common/scripts/**/*.js']                   
                },
            },
        },
        uglify: {
            options: {
                sourceMap: true,
                banner: '/*! <%= pkg.name %> <%= pkg.version %> <%= grunt.template.today("yyyy-mm-dd") %> */\n'
            },
            static_mappings: {
                files: [
                    { src: 'bundles/js/Entity/s3-Entity.js', dest: 'bundles/js/Entity/s3-Entity.min.js' },
                    { src: 'bundles/js/Correspondence/s3-Correspondence.js', dest: 'bundles/js/Correspondence/s3-Correspondence.min.js' },
                    { src: 'bundles/js/BPM/s3-BPM.js', dest: 'bundles/js/BPM/s3-BPM.min.js' },
                    { src: 'bundles/js/File/s3-File.js', dest: 'bundles/js/File/s3-File.min.js' },
                    { src: 'bundles/js/Form/s3-Form.js', dest: 'bundles/js/Form/s3-Form.min.js' },
                    { src: 'bundles/js/FormLink/s3-FormLink.js', dest: 'bundles/js/FormLink/s3-FormLink.min.js' },
                    { src: 'bundles/js/Rule/s3-Rule.js', dest: 'bundles/js/Rule/s3-Rule.min.js' },
                    { src: 'bundles/js/Scenario/s3-Scenario.js', dest: 'bundles/js/Scenario/s3-Scenario.min.js' },
                    { src: 'bundles/js/CreateNewObject/s3-CreateNewObject.js', dest: 'bundles/js/CreateNewObject/s3-CreateNewObject.min.js' },
                    { src: 'bundles/js/Report/s3-Report.js', dest: 'bundles/js/Report/s3-Report.min.js' },
                    { src: 'bundles/js/Constants/s3-Constants.js', dest: 'bundles/js/Constants/s3-Constants.min.js' },
                    { src: 'bundles/js/Tools/s3-Tools.js', dest: 'bundles/js/Tools/s3-Tools.min.js' },
                    { src: 'bundles/js/WorkFlow/s3-WorkFlow.js', dest: 'bundles/js/WorkFlow/s3-WorkFlow.min.js' },
                    { src: 'bundles/js/StartUp/s3-StartUp.js', dest: 'bundles/js/StartUp/s3-StartUp.min.js' },
                    { src: 'bundles/js/Common/s3-Common.js', dest: 'bundles/js/Common/s3-Common.min.js' }
                ],             
            }
        },
        fixmyjs: {
            options: {
                legacy: true
            },
            test: {
                files: [
                    { expand: true, cwd: 'Entity/scripts', src: ['**/*.js'], dest: 'Entity/scripts', ext: '.js' },
                    { expand: true, cwd: 'Correspondence/scripts', src: ['**/*.js'], dest: 'Correspondence/scripts', ext: '.js' },
                    { expand: true, cwd: 'BPM/scripts', src: ['**/*.js'], dest: 'BPM/scripts', ext: '.js' },
                    { expand: true, cwd: 'File/scripts', src: ['**/*.js'], dest: 'File/scripts', ext: '.js' },
                    { expand: true, cwd: 'Form/scripts', src: ['**/*.js'], dest: 'Form/scripts', ext: '.js' },
                    { expand: true, cwd: 'FormLink/scripts', src: ['**/*.js'], dest: 'FormLink/scripts', ext: '.js' },
                    { expand: true, cwd: 'Rule/scripts', src: ['**/*.js'], dest: 'Rule/scripts', ext: '.js' },
                    { expand: true, cwd: 'Scenario/scripts', src: ['**/*.js'], dest: 'Scenario/scripts', ext: '.js' },
                    { expand: true, cwd: 'CreateNewObject/scripts', src: ['**/*.js'], dest: 'CreateNewObject/scripts', ext: '.js' },
                    { expand: true, cwd: 'Report/scripts', src: ['**/*.js'], dest: 'Report/scripts', ext: '.js' },
                    { expand: true, cwd: 'Constants/scripts', src: ['**/*.js'], dest: 'Constants/scripts', ext: '.js' },
                    { expand: true, cwd: 'Tools/scripts', src: ['**/*.js'], dest: 'Tools/scripts', ext: '.js' },
                    { expand: true, cwd: 'WorkFlow/script', src: ['**/*.js'], dest: 'WorkFlow/script', ext: '.js' },
                    { expand: true, cwd: 'StartUp/scripts', src: ['**/*.js'], dest: 'StartUp/scripts', ext: '.js' },
                    { expand: true, cwd: 'Common/scripts', src: ['**/*.js'], dest: 'Common/scripts', ext: '.js' }
                ]
            }
        },
        cssmin: {
            options: {
                banner: '/* Minified on <%= grunt.template.today("yyyy-mm-dd hh:MM:ss")%> by GruntJS-cssmin */',
                sourceMap: true
            },
            target: {
                files: [{
                    expand: true,
                    cwd: 'bundles',
                    src: ['s3-custom.css'],
                    dest: 'bundles',
                    ext: '.min.css'
                }]
            }
        },
        cachebreaker: {
            QA: {
                options: {
                    match: [
                        {
                            's3-basic.min.css': 'bundles/s3-basic.min.css',
                            's3-custom.min.css': 'bundles/s3-custom.min.css',
                            's3-Common.min.js': 'bundles/js/Common/s3-Common.min.js',
                            's3-StartUp.min.js': 'bundles/js/StartUp/s3-StartUp.min.js',
                            's3-Form.min.js': 'bundles/js/Form/s3-Form.min.js',
                            's3-bpm.min.js': 'bundles/js/BPM/s3-bpm.min.js',
                            's3-Rule.min.js': 'bundles/js/Rule/s3-Rule.min.js',
                            's3-Entity.min.js': 'bundles/js/Entity/s3-Entity.min.js',
                            's3-Scenario.min.js': 'bundles/js/Scenario/s3-Scenario.min.js',
                            's3-CreateNewObject.min.js': 'bundles/js/CreateNewObject/s3-CreateNewObject.min.js',
                            's3-Report.min.js': 'bundles/js/Report/s3-Report.min.js',
                            's3-Correspondence.min.js': 'bundles/js/Correspondence/s3-Correspondence.min.js',
                            's3-file.min.js': 'bundles/js/File/s3-file.min.js',
                            's3-Constants.min.js': 'bundles/js/Constants/s3-Constants.min.js',
                            's3-FormLink.min.js': 'bundles/js/FormLink/s3-FormLink.min.js',
                            's3-Tools.min.js': 'bundles/js/Tools/s3-Tools.min.js',
                            's3-WorkFlow.min.js': 'bundles/js/WorkFlow/s3-WorkFlow.min.js'
                        }
                    ],
                    replacement: 'md5'
                },
                files: {
                    src: ['mainPage.html']
                }
            }
        },
        processhtml: {
            dist: {                
                files: {
                    'mainPage.html': ['mainPage.html']
                }
            }
        }
    });

    grunt.loadNpmTasks('grunt-contrib-clean');
    grunt.loadNpmTasks('grunt-fixmyjs');
    grunt.loadNpmTasks('grunt-contrib-uglify');
    grunt.loadNpmTasks('grunt-contrib-cssmin');
    grunt.loadNpmTasks('grunt-contrib-concat');
    grunt.loadNpmTasks('grunt-cache-breaker');
    grunt.loadNpmTasks('grunt-processhtml');

    grunt.registerTask('Debug', []);

    grunt.registerTask('Release', ['clean', 'concat', 'cssmin', 'uglify', 'processhtml', 'cachebreaker']);

};