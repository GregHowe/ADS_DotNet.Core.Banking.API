namespace Common.Api.Controller{
    using System.Collections.Generic;
    using Common.Application.Dto;
    using Microsoft.AspNetCore.Http;

public class ResponseHandler {
	public ResponseDto getOkCommandResponse(string message) {
		ResponseDto responseDto = new ResponseDto();
		ResponseOkCommandDto responseOkCommandDto = new ResponseOkCommandDto();
		responseOkCommandDto.httpStatus = StatusCodes.Status201Created;
		responseOkCommandDto.message =message;
		responseDto.response = responseOkCommandDto;
		return responseDto;
    }
	
	public ResponseDto getAppCustomErrorResponse(string errorMessages)
    {
		ResponseDto responseDto = new ResponseDto();
		string[] errors = errorMessages.Split(",");
		List<ErrorDto> errorsDto = new List<ErrorDto>();
        foreach (string error in errors) {
            errorsDto.Add(new ErrorDto(error));
        }
        ResponseErrorDto responseErrorDto = new ResponseErrorDto(errorsDto);
        responseErrorDto.httpStatus = StatusCodes.Status400BadRequest;
        responseDto.response = responseErrorDto;
        return responseDto;
    }
	
	public ResponseDto getAppExceptionResponse()
    {
		ResponseDto responseDto = new ResponseDto();
		List<ErrorDto> errorsDto = new List<ErrorDto>();
        errorsDto.Add(new ErrorDto("Server error"));
        ResponseErrorDto responseErrorDto = new ResponseErrorDto(errorsDto);
        responseErrorDto.httpStatus = StatusCodes.Status500InternalServerError;
        responseDto.response = responseErrorDto;
        return responseDto;
    }
}

}