﻿namespace MoviesDB.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Mime;
    using System.Web.Mvc;
    using Helpers;

    using MoviesDB.Domain.Services;
    using MoviesDB.Web.ViewModels;

    public class MoviesController : Controller
    {
        private const string GRID_PARTIAL_PATH = "_MoviesGrid";
        private const string DETAILS_PARTIAL_PATH = "_View";
        private const string CREATE_PARTIAL_PATH = "_Add";
        private const string EDIT_PARTIAL_PATH = "_Edit";

        private readonly IMoviesService moviesService;
        private readonly IGridMvcHelper gridMvcHelper;
        private readonly IMoviesDBConfiguration config;
        private readonly IFileHelper fileHelper;

        public MoviesController(
            IMoviesService moviesService, 
            IGridMvcHelper gridMvcHelper, 
            IMoviesDBConfiguration config, 
            IFileHelper fileHelper)
        {
            if (moviesService == null)
            {
                throw new ArgumentNullException(nameof(moviesService));
            }

            if (gridMvcHelper == null)
            {
                throw new ArgumentNullException(nameof(gridMvcHelper));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (fileHelper == null)
            {
                throw new ArgumentNullException(nameof(fileHelper));
            }

            this.moviesService = moviesService;
            this.gridMvcHelper = gridMvcHelper;
            this.config = config;
            this.fileHelper = fileHelper;
        }

        public ActionResult Index()
        {
            this.moviesService.All();
            return View();
        }

        [ChildActionOnly]
        public ActionResult GetGrid()
        {
            var items = GetMoviesAsMovieViewModels().AsQueryable().OrderBy(x => x.Id);
            var grid = this.gridMvcHelper.GetAjaxGrid(items, this.config.GridPageSize);
            return PartialView(GRID_PARTIAL_PATH, grid);
        }

        [HttpGet]
        public ActionResult GridPager(int? page)
        {
            var items = this.GetMoviesAsMovieViewModels().AsQueryable().OrderBy(x => x.Id);
            var grid = this.gridMvcHelper.GetAjaxGrid(items, page, this.config.GridPageSize);
            object jsonData = this.gridMvcHelper.GetGridJsonData(grid, GRID_PARTIAL_PATH, this);
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult Details(int id)
        {
            var moview = this.moviesService.GetById(id);
            var viewModel = MovieViewModel.FromMovie(moview);
            return this.PartialView(DETAILS_PARTIAL_PATH, viewModel);
        }

        [HttpGet]
        public PartialViewResult Create()
        {
            return this.PartialView(CREATE_PARTIAL_PATH, new MovieViewModel());
        }

        [HttpPost]
        public ActionResult Create(MovieViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.PartialView(CREATE_PARTIAL_PATH, model);
            }

            this.moviesService.Add(model.Title, model.Director, model.ReleaseDate);
            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }

        [HttpGet]
        public PartialViewResult Edit(int id)
        {
            var movie = this.moviesService.GetById(id);
            var model = MovieViewModel.FromMovie(movie);
            return this.PartialView(EDIT_PARTIAL_PATH, model);
        }

        [HttpPost]
        public ActionResult Edit(MovieViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.PartialView(EDIT_PARTIAL_PATH, model);
            }

            var movie = MovieViewModel.ToMovie(model);
            this.moviesService.Update(movie);
            return new HttpStatusCodeResult(HttpStatusCode.Accepted);
        }

        [HttpGet]
        public ActionResult Export()
        {
            var allMovies = this.moviesService.All();
            var cd = new ContentDisposition
            {
                FileName = "Movies.js",                
                Inline = false,
            };
            this.Response.AppendHeader("Content-Disposition", cd.ToString());
            return this.fileHelper.GetJsonFile(allMovies, true);            
        }

        private IEnumerable<MovieViewModel> GetMoviesAsMovieViewModels()
        {
            return this.moviesService.All().Select(MovieViewModel.SelectFromMovie);
        }
    }
}