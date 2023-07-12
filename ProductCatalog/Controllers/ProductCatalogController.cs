﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Services;
using System.Net.Mime;

namespace ProductCatalog.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/product/")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(
            IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Get the given product
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/product/{id}
        /// 
        /// </remarks>
        /// <param name="id">Product id</param>
        /// <response code="200">Product details</response>

        [HttpGet]
        [Route("product")]
        public async Task<IActionResult> GetAllProducts()
        {
            var dtos = await _productService.GetAllProductsAsync();
            return Ok(dtos);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ProductDetailsResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProduct(
            [FromRoute] Guid id)
        {
            var dto = await _productService.GetProductAsync(id);
            return Ok(dto);
        }

        [HttpPost]
        [Route("product")]
        public async Task<IActionResult> AddProduct(
            [FromBody] CreateProductRequest request)
        {
            Guid productId = await _productService.CreateProductAsync(request);

            Response.Headers.Add("Location", productId.ToString());
            return NoContent();
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateProduct(
            [FromRoute] Guid id,
            [FromBody] UpdateProductRequest request)
        {
            await _productService.UpdateProductAsync(id, request);
            return NoContent();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteProduct(
            [FromRoute] Guid id)
        {
            await _productService.DeleteProductAsync(id);
            return Ok();
        }
    }
}
