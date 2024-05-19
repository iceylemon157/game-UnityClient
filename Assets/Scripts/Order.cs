using System;
using UnityEngine.Serialization;

public class Order {
    public int OrderID;
    public RecipeSO RecipeSO;
    public int OrderScore;
    public int ExistedTime;
    
    [Serializable]
    public class OrderInfo {
        public int orderID;
        public int recipeID;
        public int orderScore;
        public int existedTime;
    }
    
    public OrderInfo GetOrderInfo() {
        return new OrderInfo {
            orderID = OrderID,
            recipeID = RecipeSO.id,
            orderScore = OrderScore,
            existedTime = ExistedTime
        };
    }
}