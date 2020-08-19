using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiBank_A3.Views
{
    public class Paginator<T>
    {
        private int PAGINATION_SIZE = 4;

        public int prevPage;
        public int currentPage;
        public int nextPage;
        public List<T> partialList;

        //TODO; use lazy loading
        public Paginator(List<T> list, int pageNo)
        {
            //pagination spaghetti
            prevPage = pageNo - 1;
            currentPage = pageNo;
            nextPage = pageNo + 1;
            //if negative
            if (pageNo <= 0)
            {
                goto ErrorCleanup;
            }
            int length = PAGINATION_SIZE;
            int startIndex = (pageNo-1) * PAGINATION_SIZE;
            //if overflow
            if (startIndex >= list.Count())
            {
                goto ErrorCleanup;
            }
            //if first page
            if (pageNo == 1)
            {
                prevPage = -1;
            }
            //if only one page
            if (list.Count() < PAGINATION_SIZE)
            {
                length = list.Count();
                nextPage = -1;
            }
            //if last page
            if (pageNo * PAGINATION_SIZE > list.Count())
            {
                length = list.Count() % PAGINATION_SIZE;
            }
            //if next page overflows
            if (pageNo * PAGINATION_SIZE >= list.Count())
            {
                nextPage = -1;
            }
            //apply pagination
            partialList = list.GetRange(startIndex, length);
            return;
        ErrorCleanup:
            //unset anything we set
            currentPage = -1;
            nextPage = -1;
            prevPage = -1;
            partialList = null;
        }

    }
}
