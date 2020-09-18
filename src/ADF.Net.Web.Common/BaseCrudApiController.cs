﻿using System;
using ADF.Net.Core.Enums;
using ADF.Net.Core.Exceptions;
using ADF.Net.Core.Globalization;
using ADF.Net.Service;
using ADF.Net.Service.GenericCrudModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ADF.Net.Web.Common
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class BaseCrudApiController<T> : ControllerBase where T : class, IServiceModel, new()
    {

        private readonly ICrudService<T> _service;

        public BaseCrudApiController(ICrudService<T> service)
        {
            _service = service;
        }

        [HttpGet]

        public ActionResult<ListModel<T>> Get()
        {
            ListModel<T> model;

            try
            {
                var filterModel = new FilterModel
                {
                    StartDate = DateTime.Now.AddYears(-2),
                    EndDate = DateTime.Now,
                    Status = StatusOption.All.GetHashCode(),
                    PageNumber = 1,
                    PageSize = 10,
                    Searched = string.Empty
                };
                model = _service.List(filterModel);
                return Ok(model);
            }

            catch (Exception exception)
            {
                model = new ListModel<T>
                {
                    HasError = true,
                    Message = exception.Message
                };
                return BadRequest(model);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<DetailModel<T>> Get(Guid id)
        {
            try
            {
                return Ok(_service.Detail(id));
            }

            catch (NotFoundException)
            {
                return NotFound(Messages.DangerRecordNotFound);
            }

            catch (Exception exception)
            {
                ModelState.AddModelError("ErrorMessage", exception.ToString());
                return BadRequest(ModelState);
            }
        }

        [HttpPost]
        public ActionResult<AddModel<T>> Post([FromBody] AddModel<T> addModel)
        {
            try
            {
                return Ok(_service.Add(addModel));
            }

            catch (ValidationException exception)
            {
                var validationResult = exception.ValidationResult;
                foreach (var t in validationResult)
                {
                    ModelState.AddModelError(t.PropertyName, t.ErrorMessage);
                }
                return BadRequest(ModelState);
            }

            catch (Exception exception)
            {
                ModelState.AddModelError("ErrorMessage", exception.Message);
                return BadRequest(ModelState);
            }
        }


        [HttpPut]
        public ActionResult Put([FromBody] UpdateModel<T> updateModel)
        {
            try
            {
                return Ok(_service.Update(updateModel));
            }

            catch (ValidationException exception)
            {
                var validationResult = exception.ValidationResult;
                foreach (var t in validationResult)
                {
                    ModelState.AddModelError(t.PropertyName, t.ErrorMessage);
                }
                return BadRequest(ModelState);
            }

            catch (NotFoundException)
            {
                ModelState.AddModelError("ErrorMessage", Messages.DangerRecordNotFound);
                return BadRequest(ModelState);
            }

            catch (Exception exception)
            {
                ModelState.AddModelError("ErrorMessage", exception.Message);
                return BadRequest(ModelState);
            }
        }
    

        [HttpDelete]
        public ActionResult Delete(Guid id)
        {
            try
            {
                _service.Delete(id);
                return Ok();
            }

            catch (InvalidTransactionException exception)
            {
                ModelState.AddModelError("ErrorMessage", exception.Message);
                return BadRequest(ModelState);
            }

            catch (Exception)
            {
                ModelState.AddModelError("ErrorMessage", Messages.DangerRecordNotFound);
                return BadRequest(ModelState);
            }
        }
    }
}
