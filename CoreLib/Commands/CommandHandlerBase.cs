using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Commands
{

    /// <summary>
    /// 結果を返さないコマンドハンドラーの基底クラス
    /// </summary>
    /// <typeparam name="TCommand">コマンドの型</typeparam>
    public abstract class CommandHandlerBase<TCommand> : IRequestHandler<TCommand>
        where TCommand : ICommand
    {
        public abstract Task Handle(TCommand request, CancellationToken cancellationToken);
    }

    /// <summary>
    /// 結果を返すコマンドハンドラーの基底クラス
    /// </summary>
    /// <typeparam name="TCommand">コマンドの型</typeparam>
    /// <typeparam name="TResult">結果の型</typeparam>
    public abstract class CommandHandlerBase<TCommand, TResult> : IRequestHandler<TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
        public abstract Task<TResult> Handle(TCommand request, CancellationToken cancellationToken);
    }

    /// <summary>
    /// クエリハンドラーの基底クラス
    /// </summary>
    /// <typeparam name="TQuery">クエリの型</typeparam>
    /// <typeparam name="TResult">結果の型</typeparam>
    public abstract class QueryHandlerBase<TQuery, TResult> : IRequestHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        public abstract Task<TResult> Handle(TQuery request, CancellationToken cancellationToken);
    }
}
