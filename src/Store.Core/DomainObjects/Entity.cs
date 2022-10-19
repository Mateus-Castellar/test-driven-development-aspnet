using Store.Core.Messages;

namespace Store.Core.DomainObjects
{
    public abstract class Entity
    {
        public Guid Id { get; }
        private List<Event> _notificacoes;
        public IReadOnlyCollection<Event> Notificacoes => _notificacoes.AsReadOnly();

        public Entity()
        {
            Id = Guid.NewGuid();
        }

        public void AdicionarEvento(Event evento)
        {
            _notificacoes ??= new();
            _notificacoes.Add(evento);
        }

        public void RemoverEvento(Event evento)
        {
            _notificacoes.Remove(evento);
        }

        public void LimparEventos()
        {
            _notificacoes?.Clear();
        }
    }
}