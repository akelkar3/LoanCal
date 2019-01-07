using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoanCal;
using LoanCal.Models;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Results;

namespace LoanCalTest
{
    [TestClass]
    public class LoanCalTest1
    {
        [TestMethod]
        public void Calculate_ShouldGenerateCorrectCashflowCount()
        {
            List<LoanDetail> testLoan = GetTestLoans();
            var controller = new LoanCal.Controllers.LoanController();
            var result = controller.Calculate(testLoan) as OkNegotiatedContentResult<List<List<CashFlow>>>;
            Assert.AreEqual((testLoan.Count + 1), result.Content.Count);
        }
        [TestMethod]
        public void Calculate_ShouldGiveEndingRemainingBalanceZero()
        {
            var testLoan = GetTestLoans();
            var controller = new LoanCal.Controllers.LoanController();
            var result = controller.Calculate(testLoan) as OkNegotiatedContentResult< List<List<CashFlow>>>;
            bool test = true;
           foreach(var item in result.Content)
            {
                if(Convert.ToInt32(item[item.Count - 1].Balance) != 0)
                {
                    test = false;
                }
            }
            Assert.IsTrue(test);

        }
        private List<LoanDetail> GetTestLoans()
        {
            List<LoanDetail> retList = new List<LoanDetail>();
            retList.Add(new LoanDetail { Balance = 1200, Rate = 10, Term = 20 });
            retList.Add(new LoanDetail { Balance = 1200, Rate = 10, Term = 20 });
            return retList;
        }
    }
}
