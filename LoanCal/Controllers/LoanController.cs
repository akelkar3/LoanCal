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
        public IHttpActionResult Calculate(List<LoanDetail> loanList)
        {
            List<List<CashFlow>> retList = new List<List<CashFlow>>();
            //there needs to be a pool loan added at the end of list
            List<CashFlow> poolLoan = new List<CashFlow>();
            foreach (LoanDetail item in loanList)
            {

                double denominator = 1 - Math.Pow((1 + item.Rate / 1200), -item.Term);
                //calculate total months payment
                double TMP = ((item.Balance) * (item.Rate / 1200)) / denominator;
                List<CashFlow> loan = new List<CashFlow>();
                for (int i = 0; i < item.Term; i++)
                {
                    CashFlow thisMonth = new CashFlow();
                    thisMonth.Balance = i == 0 ? item.Balance : loan[i - 1].Balance;
                    //calculate intrest payment
                    thisMonth.Intrest = thisMonth.Balance * item.Rate / 1200;
                    //calculate principal payment
                    thisMonth.Principal = TMP - thisMonth.Intrest;
                    //calculate balance 
                    thisMonth.Balance -= thisMonth.Principal;
                    loan.Add(thisMonth);
                    //calculations for pool
                    if (i >= poolLoan.Count)
                    {   //for loans with different terms add the values for new month
                        poolLoan.Add(new CashFlow { Balance = thisMonth.Balance, Intrest = thisMonth.Intrest, Principal = thisMonth.Principal });
                    }
                    else
                    {
                        poolLoan[i].Balance += thisMonth.Balance;
                        poolLoan[i].Intrest += thisMonth.Intrest;
                        poolLoan[i].Principal += thisMonth.Principal;
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
