using Microsoft.AspNetCore.Mvc;
using App.Models;
using Sentry;

namespace App.Controllers;

public class ProductsController : Controller
{
    static List<Product> products = new List<Product>();

    private void CreateProductWithProfiling(Product product)
    {
        // Start a Sentry transaction for profiling
        var transaction = SentrySdk.StartTransaction("create-product", "http.request");
        
        try
        {
            // Simulate some work that will be profiled
            var validateSpan = transaction.StartChild("validate-product");
            // Simulate validation work
            Thread.Sleep(100);
            if (string.IsNullOrEmpty(product.Name))
            {
                throw new ArgumentException("Product name is required");
            }
            validateSpan.Finish();

            var addSpan = transaction.StartChild("add-product");
            // Simulate database operation
            Thread.Sleep(200);
            products.Add(product);
            addSpan.Finish();

            var notificationSpan = transaction.StartChild("send-notification");
            // Simulate notification sending
            Thread.Sleep(150);
            SentrySdk.CaptureMessage("Product created successfully", SentryLevel.Info);
            notificationSpan.Finish();
        }
        catch (Exception ex)
        {
            // Capture the exception with the transaction context
            SentrySdk.CaptureException(ex);
            throw;
        }
        finally
        {
            // Finish the transaction
            transaction.Finish();
        }
    }

    public IActionResult Index()
    {
        return View(products);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Product product)
    {
        CreateProductWithProfiling(product);
        return RedirectToAction("Index");
    }

    public IActionResult Edit(int id)
    {
        Product product = products.Find(p => p.Id == id);
        return View(product);
    }

    [HttpPost]
    public IActionResult Edit(Product product)
    {
        products[products.FindIndex(p => p.Id == product.Id)] = product;
        return RedirectToAction("Index");
    }

    public IActionResult Delete(int id)
    {
        Product product = products.Find(p => p.Id == id);
        return View(product);
    }

    [HttpPost]
    public IActionResult DeleteConfirmed(int id)
    {
        products.RemoveAt(products.FindIndex(p => p.Id == id));
        return RedirectToAction("Index");
    }
} 