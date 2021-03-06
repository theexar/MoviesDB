﻿'use strict';
var MOVIES = function () {
    var controller = 'Movies/',
        moviesGridName = 'MoviesGrid',
        pagingUrl = controller + 'GridPager',
        detailsUrl = controller + 'Details',
        addMovieUrl = controller + 'Create',
        editMovieUrl = controller + 'Edit',
        exportUrl = controller + 'Export',

        moviesGridSelector = '[data-gridname=MoviesGrid]',
        editMovieButtonSelector = '.edit-movie-button',
        viewMovieDetailsButtonSelector = '.view-movie-details-button',
        addMovieButtonSelector = '.add-movie-button',
        submitMovieButtonSelector = '.add-movie',
        exportButtonSelector = '.export-button',
        submitButton = '[type=submit]',

        modalSelector = '.modal',
        modalContentSelector = '.modal .modal-content',
        formSelector = 'form';    

    function displayModal(html) {
        $(modalContentSelector).html(html);
        $(modalSelector).modal();
        $('[name=ReleaseDate]').datepicker({ maxDate: new Date(), changeYear: true, dateFormat: 'dd/mm/yy', })
    }

    function displayDetailsModal() {
        var movieId = $(this).attr('data-id');
        $.get(detailsUrl, { id: movieId }, function (html) {
            displayModal(html);
        });
    }

    function displayAddMovieModal() {
        $.get(addMovieUrl, function (html) {            
            displayModal(html);
        });
    }

    function displayEditMovieModal() {
        var id = $(this).attr('data-id');
        $.get(editMovieUrl, { id: id }, function (html) {
            displayModal(html);
        });
    }

    function extractFormData() {
        var id = $('[name=Id]').val();
        var title = $('[name=Title]').val();
        var director = $('[name=Director]').val();
        var releaseDate = $('[name=ReleaseDate]').val();
        return {
            id: id,
            title: title,
            director: director,
            releaseDate: releaseDate
        }
    }

    function getUrlBasedOnAction(action) {
        if (action == 'add') {
            return addMovieUrl;
        } else if (action == 'edit') {
            return editMovieUrl;
        }
    }

    function submitMovie(evt) {        
        evt.preventDefault();
        var action = $(submitButton).attr('data-action');
        var url = getUrlBasedOnAction(action);
        var $form = $(this);
        var data;
        if ($form.valid()) {
            data = extractFormData();
            $.post(url, data, function (response, status, xhr) {
                if (xhr.status != 201 && xhr.status != 202) {
                    $(modalContentSelector).html(response);
                } else {
                    $(modalSelector).modal('hide');
                    pageGrids.MoviesGrid.refreshFullGrid();
                }
            });
        }
    }

    function exportMovies() {
        window.location = exportUrl;
    }

    function init() {
        $('.grid-mvc').gridmvc();
        pageGrids[moviesGridName].ajaxify({
            getData: pagingUrl,
            getPagedData: pagingUrl
        });
        $(moviesGridSelector).on('click', editMovieButtonSelector, displayEditMovieModal);
        $(moviesGridSelector).on('click', viewMovieDetailsButtonSelector, displayDetailsModal);
        $(addMovieButtonSelector).on('click', displayAddMovieModal);
        $(exportButtonSelector).on('click', exportMovies);
        $(modalSelector).on('submit', formSelector, submitMovie);
    }

    return {
        init: init
    }
}();
MOVIES.init();