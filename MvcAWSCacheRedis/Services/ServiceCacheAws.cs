using MvcAWSCacheRedis.Helpers;
using MvcAWSCacheRedis.Models;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcAWSCacheRedis.Services
{
    public class ServiceCacheAws
    {
        private IDatabase cache;

        public ServiceCacheAws()
        {
            this.cache = CacheRedisMultiplexer.Connection.GetDatabase();
        }

        // VAMOS A TENER UN METODO PARA ALMACENAR PRODUCTOS EN CACHE
        //DENTRO DEL CACHE, LO QUE ALMACENAREMOS SERÁ UNA COLECCIÓN DE PRODUCTOS EN FORMATO JSON

        public void AddProducto(Productos producto)
        {
            List<Productos> productos;
            string json = this.cache.StringGet("productoscache");
            if (json == null)
            {
                productos = new List<Productos>();
            }
            else
            {
                productos = JsonConvert.DeserializeObject<List<Productos>>(json);
            }
            productos.Add(producto);
            //vamos a serializar

            json = JsonConvert.SerializeObject(productos);
            //almacenamos en cache redis
            this.cache.StringSet("productoscache", json, TimeSpan.FromMinutes(30));

        }

        public List<Productos> GetProductosCache()
        {
            string json = this.cache.StringGet("productoscache");
            if (json == null)
            {
                return null;
            }
            else
            {
                List<Productos> productos = JsonConvert.DeserializeObject<List<Productos>>(json);
                return productos;
            }
        }

        public void EliminarProductoCache(int idProducto)
        {
            //RECUPERAMOS TODOS LOS PRODUCTOS
            List<Productos> productos = this.GetProductosCache();
            if (productos != null)
            {
                Productos producto = productos.SingleOrDefault(x => x.IdProducto == idProducto);
                productos.Remove(producto);
                //si no hay productos eliminamos la clave
                if (productos.Count() == 0)
                {
                    this.cache.KeyDelete("productoscache");
                }
                else
                {
                    //Serializamos y almacenamos los datos sin el producto eliminado
                    String json = JsonConvert.SerializeObject(productos);
                    this.cache.StringSet("productoscache", json, TimeSpan.FromMinutes(30));
                }
            }
        }
    }
}
