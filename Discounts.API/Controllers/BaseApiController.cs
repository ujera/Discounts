// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Exceptions.ResponceFormat;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")] 
    public abstract class BaseApiController : ControllerBase
    {
        protected ActionResult<ApiResponse<T>> OkResponse<T>(T data, string message = "")
        {
            return Ok(new ApiResponse<T>(data, message));
        }

        protected ActionResult<ApiResponse<T>> CreatedResponse<T>(T data, string message = "")
        {
            return StatusCode(201, new ApiResponse<T>(data, message));
        }
    }
}
