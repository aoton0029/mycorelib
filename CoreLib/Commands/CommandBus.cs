using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Commands
{
    /// <summary>
    /// コマンドバスのインターフェース
    /// </summary>
    public interface ICommandBus
    {
        /// <summary>
        /// コマンドを送信（結果なし）
        /// </summary>
        Task SendAsync(ICommand command, CancellationToken cancellationToken = default);

        /// <summary>
        /// コマンドを送信（結果あり）
        /// </summary>
        Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);

        /// <summary>
        /// クエリを送信
        /// </summary>
        Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// MediatRを利用したコマンドバス実装
    /// </summary>
    public class CommandBus : ICommandBus
    {
        private readonly IMediator _mediator;

        public CommandBus(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// コマンドを送信（結果なし）
        /// </summary>
        public Task SendAsync(ICommand command, CancellationToken cancellationToken = default)
        {
            return _mediator.Send(command, cancellationToken);
        }

        /// <summary>
        /// コマンドを送信（結果あり）
        /// </summary>
        public Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
        {
            return _mediator.Send(command, cancellationToken);
        }

        /// <summary>
        /// クエリを送信
        /// </summary>
        public Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
        {
            return _mediator.Send(query, cancellationToken);
        }
    }
}
