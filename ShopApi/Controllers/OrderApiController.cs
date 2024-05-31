using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Data;
using ShopApi.Model;
using ShopApi.Model.Dto;
using ShopApi.Repository.IRepository;

namespace ShopApi.Controllers;

[ApiController]
[Route("/api/OrderApi")]
public class OrderApiController: ControllerBase
{
    private readonly IOrderRepository _db;
    private readonly ApplicationDbContext _dbContext;
    private readonly IProductRepository _dbProduct;
    private readonly IMapper _mapper;
    private readonly ApiResponse _response;
    public OrderApiController(IOrderRepository db,IMapper mapper,ApiResponse response,IProductRepository dbProduct,ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _db = db;
        _mapper = mapper;
        _dbProduct = dbProduct;
        _response = response;
    }
    [HttpGet("getAllOrder")]
    public async Task<ActionResult<ApiResponse>> GetProducts()
    {
        try
        {
            IEnumerable<Order> orderList = await _db.GetAllAsync(includeProperties:"OrderProducts.Product");
            
            var orderDtos= _mapper.Map<List<OrderDto>>(orderList);

          

            foreach (var orderDto in orderDtos)
            {
                orderDto.Total = orderList
                    .Where(order => order.Id == orderDto.Id)
                    .Sum(order => order.OrderProducts.Sum(op => op.Total));
            }

            _response.Result = orderDtos;
            _response.StatusCode = HttpStatusCode.OK;

            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>{e.ToString()};
        
        }
    
        return _response;
    }
    [HttpGet("{id:int}", Name = "GetOrderById")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse>> GetById(int id)
    {
        try
        {
            if (id==0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;           
                return BadRequest(_response);
            }

            var order = await _db.GetAsync(u => u.Id == id,includeProperties:"OrderProducts.Product");
            if (order==null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);

            }

            _response.Result = _mapper.Map<OrderDto>(order);
            _response.IsSuccess = true;
            return Ok(_response);

        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return _response;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Roles = "customer")]
    public async Task<ActionResult<ApiResponse>> CreateOrder([FromBody]CreateOrderDto createOrder)
    {
    
        try
        {
           
           
            if ( _dbContext.ApplicationUsers.FirstOrDefault(u=>u.Id==createOrder.UserId)==null)
            {
                ModelState.AddModelError("ErrorMessage","UserId doesnt exists");
                return BadRequest(ModelState);
            }
            var existingId = await _dbProduct.GetAllAsync(c => createOrder.ProductId.Contains(c.Id));
            if (existingId.Count==0)
            {
                ModelState.AddModelError("ErrorMessages"," There is no product with this Id");
                return BadRequest(ModelState);
            }

          
            if (createOrder==null)
            {
                return BadRequest(createOrder);
            }
            
            Order order = _mapper.Map<Order>(createOrder);
            await _db.CreateAsync(order);

            for (int i=0;i<createOrder.ProductId.Count;i++)
            {
                var productId = createOrder.ProductId[i];
                var quantity = createOrder.Quantity[i];
                var product = await _dbProduct.GetAsync(u => u.Id == productId,tracked:false);
           
                if (product !=null)
                {
                    
                var orderProduct = new OrderProducts { OrderId = order.Id, ProductId = productId,Quantity = quantity,Total = quantity * product.Price };
            order.OrderProducts.Add(orderProduct);   
                    
                }
            }
              
            await _db.SaveAsync();
               
            _response.Result = _mapper.Map<OrderDto>(order);
            _response.StatusCode = HttpStatusCode.Created;
            return CreatedAtRoute("GetOrderById", new { id = order.Id }, _response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { e.ToString() };
        }
    
        return _response;
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [HttpDelete("{id:int}", Name = "DeleteOrder")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse>> DeleteOrder(int id)
    {
        try
        {
    
            if (id==0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                BadRequest(_response);
            }
    
            var order = await _db.GetAsync(u => u.Id == id,includeProperties: "OrderProducts.Product");
            if (order==null) 
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                NotFound(_response);
            }
            order.OrderProducts.Clear();
            await _db.DeleteAsync(order);
            _response.StatusCode = HttpStatusCode.NoContent;
            _response.IsSuccess = true;
            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { e.ToString() };
        }
    
        return _response;
    }
    
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "admin")]
    [HttpPut("{id:int}", Name = "UpdateOrder")]
    public async Task<ActionResult<ApiResponse>> UpdateProduct(int id,[FromBody]UpdateOrderDto updateOrder)
    {
        if (updateOrder==null || updateOrder.Id!=id)
        { 
            _response.StatusCode = HttpStatusCode.BadRequest;
            return BadRequest(_response);
        }
        var existingOrder = await _db.GetAsync(p => p.Id == id,tracked:false, includeProperties: "OrderProducts");
        if (existingOrder == null)
        {
             
            return NotFound("Order not found");
        }
      

        var existingId = await _dbProduct.GetAllAsync(c => updateOrder.ProductId.Contains(c.Id));
        if (existingId.Count==0)
        {
            ModelState.AddModelError("ErrorMessages","Product ID is invalid");
            return BadRequest(ModelState);
        }
     

         
       
        existingOrder.OrderProducts.Clear();

        Order order = _mapper.Map<Order>(updateOrder);
        
        for (int i=0;i<updateOrder.ProductId.Count;i++)
        { 
            var productId = updateOrder.ProductId[i];
            var quantity = updateOrder.Quantity[i];
            var product = await _dbProduct.GetAsync(u => u.Id == productId);
       
            if (product !=null)
            {
                    
                var orderProduct = new OrderProducts { OrderId = order.Id, ProductId = productId,Quantity = quantity,Total = quantity * product.Price };
                order.OrderProducts.Add(orderProduct);   
                    
            }
        }
        await _db.UpdateAsync(existingOrder);
       
        
        
        _response.IsSuccess = true;
        _response.StatusCode = HttpStatusCode.NoContent;
        return Ok(_response);
    }

}