using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using maho;
namespace maho { 
public class HexCellPriorityQueue  {


        List<HexCell> List = new List<HexCell>();
        int count = 0;
        int minimum = int.MaxValue;
        public int Count
        {
            get
            {
                return count;
            }
        }
        public void Enqueue (HexCell cell)
        {
            count += 1;
            int priority = cell.SearchPriority;
            if(priority < minimum)
            {
                minimum = priority;
            }
            while(priority >= List.Count)
            {
                List.Add(null);
            }
            cell.NextWithSamePriority = List[priority];
            List[priority] = cell;
        }
        public HexCell Dequeue()
        {
            count -= 1;
            for (; minimum < List.Count; minimum++)
            {
                HexCell cell = List[minimum];
                if(cell != null)
                {
                    List[minimum] = cell.NextWithSamePriority;
                    return cell;
                }
            }
            return null;
        }
        public void Change(HexCell cell,int oldPriority)
        {
            HexCell current = List[oldPriority];
            HexCell next = current.NextWithSamePriority;
            if(current == cell)
            {
                List[oldPriority] = next;
            }
            else
            {
                while(next != cell)
                {
                    current = next;
                    next = current.NextWithSamePriority;
                }
                current.NextWithSamePriority = cell.NextWithSamePriority;
            }
            Enqueue(cell);
            count -= 1;
        }
        public void Clear()
        {
            List.Clear();
            count = 0;
            minimum = int.MaxValue;
        }
	
}
}
