using DataInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.WPF.Models
{
    public static class HierarchyFactory
    {
        public static void GenerateFinance(ref DataObjectHierarchy DH)
        {
            DH.ClearItems();
            DH.Add(new DataObjectHierarchyItem() { ID = new HKey(new int[] { 1 }), Name = "Project" });

            DH.Add(new DataObjectHierarchyItem() { ID = new HKey(new int[] { 1,1 }), Name = "Transactions" });
            DH.Add(new DataObjectHierarchyItem() { ID = new HKey(new int[] { 1,1,1 }), Name = "Income" });
            DH.Add(new DataObjectHierarchyItem() { ID = new HKey(new int[] { 1,1,2}), Name = "Expense" });

            DH.Add(new DataObjectHierarchyItem() { ID = new HKey(new int[] { 1,2 }), Name = "Timelines" });

            DH.Add(new DataObjectHierarchyItem() { ID = new HKey(new int[] { 1,3 }), Name = "Simulations" });
        }
    }
}
