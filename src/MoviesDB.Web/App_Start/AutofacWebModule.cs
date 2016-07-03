﻿namespace MoviesDB.Web.App_Start
{
    using Autofac;
    using MoviesDB.DataAccessLayer;
    using MoviesDB.DataAccessLayer.Repositories;
    using MoviesDB.DataAccessLayer.UnitOfWork;
    using MoviesDB.Domain.Models;
    using MoviesDB.Domain.Repositories;
    using MoviesDB.Domain.Services;

    public class AutofacWebModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MoviesDbContext>().As<IDbContext>().InstancePerRequest();

            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerRequest();

            builder.RegisterType<GenericRepository<Movie, int>>().As<IRepository<Movie, int>>().InstancePerRequest();

            builder.RegisterType<MoviesService>().As<IMoviesService>().InstancePerRequest();

            base.Load(builder);
        }
    }
}