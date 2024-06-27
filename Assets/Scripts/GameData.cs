using System;
using System.Collections.Generic;
using UnityEngine;

public class GameData {

    public string RecipeMode;
    
    public int Round;
    public int TotalScore;
    public int TimeLeft; // TimeLeft is not implemented yet, possibly never will be
    
    public Order.OrderInfo NewOrder;
    public List<int> OrderDelivered = null;
    public int RecipeTimeout; // Timeout is not implemented yet, possibly never will be
    public List<Order.OrderInfo> OrderList = null;
    
    public Vector2 PlayerPosition;
    public List<int> PlayerHoldItems;
    
    public int FryingTimer;
    public StoveCounter.FryingState FryingState;
}