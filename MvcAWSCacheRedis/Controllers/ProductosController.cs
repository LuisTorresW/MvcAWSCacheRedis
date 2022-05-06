using Microsoft.AspNetCore.Mvc;
using MvcAWSCacheRedis.Models;
using MvcAWSCacheRedis.Repositories;
using MvcAWSCacheRedis.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcAWSCacheRedis.Controllers
{
    public class ProductosController : Controller
    {
        private RepositoryProductos repo;
        private ServiceCacheAws service;

        public ProductosController(RepositoryProductos repo,ServiceCacheAws service)
        {
            this.service = service;
            this.repo = repo;
        }


        public IActionResult Index()
        {
            List<Productos> productos = this.repo.GetProductos();
            return View(productos);
        }

        public IActionResult Details(int id)
        {
            Productos pro = this.repo.FindProducto(id);
            return View(pro);
        }

        public IActionResult SeleccionarFavorito(int id)
        {
            //BUSCAMOS EL PRODUCTO A ALMACENAR DENTRO DEL REPO
            Productos productos = this.repo.FindProducto(id);
            //almacenamos el producto en la cache
            this.service.AddProducto(productos);
            TempData["MENSAJE"] = "Producto" + productos.Nombre + "almacenado como Favorito";
            return RedirectToAction("Index");
        }

        public IActionResult Favoritos()
        {
            List<Productos> productos = this.service.GetProductosCache();
            return View(productos);
        }

        public IActionResult EliminarFavoritos(int id)
        {
            this.service.EliminarProductoCache(id);
            return RedirectToAction("Favoritos");
        }
    }
}
