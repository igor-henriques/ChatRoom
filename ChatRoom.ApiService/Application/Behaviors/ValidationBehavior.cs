namespace ChatRoom.ApiService.Application.Behaviors;

public sealed class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IValidator<TRequest>? _validator;

    public ValidationBehaviour(IValidator<TRequest>? validator = null)
    {
        _validator = validator;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validator is null)
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var result = await _validator.ValidateAsync(context, cancellationToken);

        if (!result.IsValid)
        {
            throw new ValidationException(string.Join('\n', result.Errors.Select(x => x.ErrorMessage)));
        }

        return await next();
    }
}
