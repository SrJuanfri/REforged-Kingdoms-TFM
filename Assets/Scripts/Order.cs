using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Order
{
    public enum Design
    {
        Espada,
        Azada,
        Pala,
        Daga
    }

    public enum BladeMaterial
    {
        Cobre,
        Hierro,
        Plata,
        Oro
    }

    public enum HandleMaterial
    {
        Pino,
        Abedul,
        Alamo,
        Abeto
    }

    public string orderText;
    public Design design;
    public BladeMaterial bladeMaterial;
    public HandleMaterial handleMaterial;
    public bool isCompleted;
    public bool isWeapon;
    public int price;

    // Constructor
    public Order(string orderText, Design design, BladeMaterial bladeMaterial, HandleMaterial handleMaterial, bool isCompleted, bool isWeapon, int price)
    {
        this.orderText = orderText;
        this.design = design;
        this.bladeMaterial = bladeMaterial;
        this.handleMaterial = handleMaterial;
        this.isCompleted = isCompleted;
        this.isWeapon = isWeapon;
        this.price = price;
    }

    // Métodos para establecer y obtener los valores de las variables
    public void SetOrderText(string text)
    {
        orderText = text;
    }

    public string GetOrderText()
    {
        return orderText;
    }

    public void SetDesign(Design design)
    {
        this.design = design;
    }

    public Design GetDesign()
    {
        return design;
    }

    public void SetBladeMaterial(BladeMaterial bladeMaterial)
    {
        this.bladeMaterial = bladeMaterial;
    }

    public BladeMaterial GetBladeMaterial()
    {
        return bladeMaterial;
    }

    public void SetHandleMaterial(HandleMaterial handleMaterial)
    {
        this.handleMaterial = handleMaterial;
    }

    public HandleMaterial GetHandleMaterial()
    {
        return handleMaterial;
    }

    public void SetIsCompleted(bool isCompleted)
    {
        this.isCompleted = isCompleted;
    }

    public bool GetIsCompleted()
    {
        return isCompleted;
    }

    public void SetIsWeapon(bool isWeapon)
    {
        this.isWeapon = isWeapon;
    }

    public bool GetIsWeapon()
    {
        return isWeapon;
    }

    public void SetPrice(int price)
    {
        this.price = price;
    }

    public int GetPrice()
    {
        return price;
    }

    public static Order FromOrderData(OrderData orderData)
    {
        return new Order(
            orderData.orderText,
            orderData.design,
            orderData.bladeMaterial,
            orderData.handleMaterial,
            orderData.isCompleted,
            orderData.isWeapon,
            orderData.price
        );
    }

    // Método para convertir de Order a OrderData
    public OrderData ToOrderData()
    {
        OrderData orderData = new OrderData();
        orderData.orderText = this.GetOrderText();
        orderData.design = this.GetDesign();
        orderData.bladeMaterial = this.GetBladeMaterial();
        orderData.handleMaterial = this.GetHandleMaterial();
        orderData.isCompleted = this.GetIsCompleted();
        orderData.isWeapon = this.GetIsWeapon();
        orderData.price = this.GetPrice();
        return orderData;
    }
}