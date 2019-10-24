namespace StartupManager.Commands
{
    public interface ICommandHandler<TOut, TIn>
    {
         TOut Run(TIn args);
    }
}