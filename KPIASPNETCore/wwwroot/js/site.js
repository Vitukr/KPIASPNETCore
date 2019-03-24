// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var app = new Vue({
    el: '#app',
        data: {
        location: null,
        name: null,
        prename: null,
        comment: null,
        //topictures: false,
        isInitial: true,
        isSaving: false,
        filesnum: 0,
        info: 'testinfo',
        rating: 0,
        faces: null,
        images: null,
        suggest: null,
        suggestions: null,
        showsuggest: false
    },
    mounted: function (event) {
            this.location = window.location.origin;            
            this.name = localStorage.getItem('vuename');
            this.getimages();
    },
    updated() {
            
    },
    computed: {

    },
    methods: {
        popupimg: function (event) {
            console.log('modal');
            var modal = document.getElementById('imgmodal');
            modal.style.display = "block";
            var img = document.getElementById('imgpopup');
            img.src = event.target.src;
            //this.$refs.popuppic.
        },
        closemodal: function (event) {
            console.log('closemodal');
            var modal = document.getElementById('imgmodal');
            modal.style.display = "none";
        },
        setRate: function ( event) {
            var star = event.target.value;
            var id = event.target.name;
            console.log('Star: ' + star);
            console.log('Id: ' + id);
            axios.get(window.location.origin + '/api/FileUpload/SetRate?id=' + id + '&rate=' + star)
                    .then(() => {})
                    .catch(err => {alert(err); });
        },
        submitsugest: function (event) {
            var bodyFormData = new FormData();
            bodyFormData.set('name', this.name);
            bodyFormData.set('suggest', this.suggest);
            axios.post(window.location.origin + '/api/FileUpload/PostSuggestion', bodyFormData)
                    .then(() => {this.getsuggests(); this.showsuggest = true; })
                    .catch(err => {alert(err); });
        },
        getsuggests: function () {
            $.getJSON(window.location.origin + '/api/FileUpload/GetSuggests', function (result) {
                this.suggestions = result;
            }.bind(this));
        },
        setshowsuggest: function () {
            this.showsuggest = !this.showsuggest;
            if (this.showsuggest) {
                this.getsuggests();
            }
        },
        submitname() {
            if (this.prename !== null) {
                this.name = this.prename;
                localStorage.setItem('vuename', this.name);
                this.topictures = true;
                this.getimages();
            }
            else {
                this.topictures = false;
            }
        },
        save(formData) {
            // upload data to the server
            this.uploadImages(formData)
            .then(() => {
                this.isInitial = true;
                this.isSaving = false;
                this.getimages();
            })
            .catch(err => {
                alert(err);
                this.isInitial = true;
                this.isSaving = false;
            });
        },
        filesChanged: function (event) {
            this.isInitial = false;
            this.isSaving = true;
            var files = event.target.files;
            this.filesnum = event.target.files.length;
            const formData = new FormData();

            if (files.length > 0) {
                Array
                    .from(Array(files.length).keys())
                    .map(x => {
                        if (files[x].type.includes("image")) {
                            formData.append("files", files[x]);
                        }
                });

                // save it
                var numfiles = 0;
                for (var key of formData.keys()) {
                    numfiles++;
                }
                if (numfiles > 0) {
                    this.save(formData);
                }
                else {
                alert('Not image file(s).');
                this.isInitial = true;
                this.isSaving = false;
                }
            }
        },
        getimages: function () {
            this.faces = [];
            $.getJSON(window.location.origin + '/api/FileUpload/GetImages', function (result) {
                this.faces = result;
                for (let i = 0; i < this.faces.length; i++) {
                    this.faces[i].fileName = window.location.origin + '/imagestest/' + this.faces[i].fileName;
                    this.faces[i].star1 = this.faces[i].id + 'star1';
                    this.faces[i].star2 = this.faces[i].id + 'star2';
                    this.faces[i].star3 = this.faces[i].id + 'star3';
                    this.faces[i].star4 = this.faces[i].id + 'star4';
                    this.faces[i].star5 = this.faces[i].id + 'star5';
   
                    console.log(this.faces[i].rate);
                }
            }.bind(this)); // .bind(this)
        },
        uploadImages: function (formData) {
            return axios.post(window.location.origin + '/api/FileUpload/Uploadfile?name=' + this.name, formData);
        },
        allowdrop: function (event) {
            event.preventDefault();
        },
        allowdrag: function (event) {
        },
        dragfile: function (event) {
            event.dataTransfer.setData("delimg", event.target.src);
            this.info = event.target.src;
        },
        deletefile: function (event) {
            event.preventDefault();
            var fileName = event.dataTransfer.getData("delimg");
            console.log('fileName delete: ' + fileName);
   
            axios.get(window.location.origin + '/api/FileUpload/Deletefile?fileName=' + fileName)
                    .then(() => {this.getimages(); })
                    .catch(err => {alert(err); });
        },
    },
});

