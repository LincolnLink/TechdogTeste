using Dev.Business.Notificacoes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dev.Business.Interfaces
{
    public interface INotificador
    {
        /// <summary> Retorna se existe notificações na lista.</summary> 
        bool TemNotificacao();

        /// <summary> Retorna a lista de notificação.</summary>
        List<Notificacao> ObterNotificacoes();

        /// <summary> Adiciona a notificação na lista.</summary> 
        void Handle(Notificacao notificacao);
    }
}
