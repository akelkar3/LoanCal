using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading;
using System.Threading.Tasks;
using LoanCal.Models;
using System.Web.Http.Description;

namespace LoanCal.Controllers
{
    public class LoanController : ApiController
    {
        [ResponseType(typeof(List<List<CashFlow>>))]
        [HttpPost]
        public async Task<IHttpActionResult> Calculate(List<LoanDetail> loanList)
        {
            List<List<CashFlow>> retList = new List<List<CashFlow>>();
            // test data
            //retList.Add(new CashFlow { Balance = 120, Intrest = 10, Principal = 20 });
            //retList.Add(new CashFlow { Balance = 90, Intrest = 10, Principal = 20 });
            //retList.Add(new CashFlow { Balance = 60, Intrest = 10, Principal = 20 });
            //retList.Add(new CashFlow { Balance = 30, Intrest = 10, Principal = 20 });
            //retList.Add(new CashFlow { Balance = 0, Intrest = 10, Principal = 20 });
            List<CashFlow> poolLoan = new List<CashFlow>();
            foreach (LoanDetail item in loanList)
            {

                double denominator = 1 - Math.Pow((1 + item.Rate / 1200), -item.Term);
                double TMP = ((item.Balance) * (item.Rate / 1200)) / denominator;
                List<CashFlow> loan = new List<CashFlow>();
                for (int i = 0; i < item.Term; i++)
                {
                    CashFlow thisMonth = new CashFlow();
                    thisMonth.Balance = i == 0 ? item.Balance :0;
                    if (i==0)
                    {
                        thisMonth.Balance = item.Balance;
                    }
                    else
                    {
                        thisMonth.Balance = loan[i - 1].Balance;
                    }
                    thisMonth.Intrest = thisMonth.Balance * item.Rate / 1200;
                    thisMonth.Principal = TMP - thisMonth.Intrest;
                    thisMonth.Balance  -= thisMonth.Principal;
                    
                    loan.Add(thisMonth);
                    //calculations for pool
                    if (i >= poolLoan.Count)
                    {
                        
                        poolLoan.Add( new CashFlow { Balance= thisMonth.Balance, Intrest=thisMonth.Intrest,Principal=thisMonth.Principal });
                    }
                    else
                    {
                        poolLoan[i].Balance = poolLoan[i].Balance+ thisMonth.Balance;
                        poolLoan[i].Intrest = poolLoan[i].Intrest + thisMonth.Intrest;
                        poolLoan[i].Principal = poolLoan[i].Principal+ thisMonth.Principal;

                    }

                }   
                retList.Add(loan);
              
            }
            retList.Add(poolLoan);
            if (!ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            return this.Ok<List<List<CashFlow>>>(retList);
        }
    }
}
