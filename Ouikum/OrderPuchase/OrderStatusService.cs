using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ouikum.OrderPuchase
{
    #region enum
    public enum OrderStatusAction
    {
        All, ConfirmBySC, Admin, Important, Sentbox, Trash, myRequest, Draft, FrontEnd, CountOrderPurchase
    }
    #endregion

    public class OrderStatusService : BaseSC
    {
        public string CreateWhereAction(OrderStatusAction action, int? ToCompID = 0)
        {
            var sqlWhere = string.Empty;

            if (action == OrderStatusAction.ConfirmBySC)
            {
                sqlWhere = "Status = 'B' and Type = 'SC'";
            }

            return sqlWhere;
        }
    }
}
