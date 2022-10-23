using HtmlAgilityPack;
using System.Net;
using CarMiner.Context;
using CarMiner.Entities;
using System.Text.RegularExpressions;

// the program is still in work, I want to rewrite to the form which a program should be look like
public class Otomoto
{
    public static void Main(string[] args)
    {
        
        
        string html = "https://www.otomoto.pl/osobowe";
        string path = "D:\\Projects\\CarMiner\\Photos\\";
        var offers = GetMainSite(html);
        
        // here open each offer separate
        foreach (string offerHtml in offers)
        {
            var start = GetTime();
            Console.WriteLine(offerHtml);
            // again what above
            while (true)
            {
                
                Dictionary<string, string> carParameters = new Dictionary<string, string>(); // this dict is to store collected data of current offer
                long end = GetTime();

                if (end - start > 2000)
                {
                    HtmlWeb web = new();
                    var doc = web.Load(offerHtml);
                    IEnumerable<HtmlNode> id = doc.DocumentNode.Descendants(0).Where(n => n.HasClass("offer-meta__value")); // id value
                    string offerId = id.ElementAt(1).InnerText;
                    carParameters.Add("offer_id", offerId);
                    if (Directory.Exists(path + offerId))
                    {
                        break;
                    }
                    
                    var urls = doc.DocumentNode.Descendants(0).Where(n => n.HasClass("offer-content__gallery"))
                        .ElementAt(0).Descendants("img")
                        .Select(e => e.GetAttributeValue("src", null))
                        .Where(s => !String.IsNullOrEmpty(s));
                    int price = GetPrice(doc);
                    //Creating directory for photos
                    Directory.CreateDirectory(path + offerId);
                    //Downloading photos to dir by offer_id
                    //download_images(urls, path, offerId);

                    foreach (HtmlNode node in doc.DocumentNode.Descendants(0).Where(n => n.HasClass("offer-params__list")))
                    {
                        int ctn1 = 0;
                        foreach (HtmlNode htmlNode in node.Descendants(0).Where(n => n.HasClass("offer-params__label")))
                        {
                            string nameOfLabel = htmlNode.InnerText.ToLower();
                            Console.WriteLine(htmlNode.InnerText);
                            string[] keyWords = { "marka pojazdu", "model pojazdu", "rok produkcji", "przebieg" ,"rodzaj paliwa", "moc" };
                            if (keyWords.Contains(nameOfLabel))
                            {
                                carParameters.Add(nameOfLabel, ReplaceWhitespace(node.Descendants(0)
                                    .Where(n => n.HasClass("offer-params__value"))
                                    .ElementAt(ctn1).InnerText, ""));
                            }
                            ctn1++;
                        }
                    }

                    using (carsContext db = new carsContext())
                    {
                        if (!db.Models.Any(b => b.Modelname == carParameters["model pojazdu"]) &
                            !db.Brands.Any(b => b.Brandname == carParameters["marka pojazdu"]))
                        {
                            Brand brand = new Brand { Brandname = carParameters["marka pojazdu"] };
                            AddCarBrand(db, brand);
                    
                            brand = db.Brands.FirstOrDefault(idadd =>
                                idadd.Brandname == carParameters["marka pojazdu"])!;
                            Model model = new Model
                                { Modelname = carParameters["model pojazdu"], IdbrandNavigation = brand };
                            AddCarModel(db, model);
                    
                            model = db.Models.FirstOrDefault(idadd =>
                                idadd.Modelname == carParameters["model pojazdu"])!;
                            AddCar(db, carParameters, offerId, brand, model, price);
                        }
                        // if brand exist but model not 
                        else if (!db.Models.Any(b => b.Modelname == carParameters["model pojazdu"]) &
                                 db.Brands.Any(b => b.Brandname == carParameters["marka pojazdu"]))
                        {
                            Brand brand = db.Brands.FirstOrDefault(idadd =>
                                idadd.Brandname == carParameters["marka pojazdu"])!;
                            Model model = new Model
                                { Modelname = carParameters["model pojazdu"], IdbrandNavigation = brand };
                            AddCarModel(db, model);
                    
                            model = db.Models.FirstOrDefault(idadd =>
                                idadd.Modelname == carParameters["model pojazdu"])!;
                            AddCar(db, carParameters, offerId, brand, model, price);
                        }
                        else
                        {
                            Brand brand = db.Brands.FirstOrDefault(idadd =>
                                idadd.Brandname == carParameters["marka pojazdu"])!;
                            Model model = db.Models.FirstOrDefault(idadd =>
                                idadd.Modelname == carParameters["model pojazdu"])!;
                            AddCar(db, carParameters, offerId, brand, model, price);
                        }
                    }

                    // If the task ends successful, go to next offer
                    break;
                }
            }
        }

    }

    private static readonly DateTime JanFirst1970 = new DateTime(1970, 1, 1);
    private static long GetTime()
    {
        return (long)((DateTime.Now.ToUniversalTime() - JanFirst1970).TotalMilliseconds + 0.5);
    }

    // func for downloading images
    public static void download_images(IEnumerable<string> urls, string path, string id)
    {
        int ctn = 0;
        //make async
        foreach (var url in urls)
        {
            WebClient client = new WebClient();
            string _url = url.Remove(url.Count() - 7) + "1080x720";
            //Console.WriteLine(_url);
            client.DownloadFile(new Uri(_url), String.Format(@"{0}{1}\{2}.jpg", path, id, ctn.ToString()));
            ctn++;
        }
    }
    private static readonly Regex SWhitespace = new Regex(@"\s+");
    private static string ReplaceWhitespace(string input, string replacement)
    {
        return SWhitespace.Replace(input, replacement);
    }

    private static void AddCar(carsContext db, Dictionary<string,string> carParameters, string offerId, Brand brand, Model model, int price)
    {
        
            db.Adds.Add(new Add
            {
                Idotomoto = Convert.ToInt64(offerId),
                IdbrandNavigation = brand,
                IdmodelNavigation = model,
                Prodyear = Convert.ToInt32(carParameters["rok produkcji"]),
                Fuel = carParameters["rodzaj paliwa"],
                Mileage = carParameters["przebieg"],
                Power = carParameters["moc"],
                //Price = price
            });
            db.SaveChanges();
        
    }
    
    private static void AddCarBrand(carsContext db, Brand brand)
    {
        db.Brands.Add(brand);
            db.SaveChanges();
    }
    
    private static void AddCarModel(carsContext db, Model model)
    {
        db.Models.Add(model);
        db.SaveChanges();
    }

    private static List<string> GetMainSite(string html)
    {
        long start = GetTime();
        HtmlWeb web = new();
        List<string> offers = new();
        // this code get's us the html of the site
        // the while loop is to emilinate chance to get null response
        while (true)
        {
            long end = GetTime();
            if (end - start > 2000)
            {
                var nodes = web.Load(html).DocumentNode.SelectNodes("//a[@href]");
                
                if (nodes != null)
                {
                    foreach (var link in nodes)
                    {
                        string hrefValue = link.GetAttributeValue("href", string.Empty);
                        
                        if (hrefValue.Trim().StartsWith("https://www.otomoto.pl/oferta/") & !offers.Contains(hrefValue))
                        {
                            offers.Add(hrefValue);
                        }
                    }
                    break;
                }
            }
        }

        return offers;
    }

    private static int GetPrice(HtmlDocument doc)
    {
        var price = doc.DocumentNode.Descendants(0).Where(n => n.HasClass("offer-price__number"));
        var priceString = price.ElementAt(1).InnerText.Replace(" ", "");
        priceString = priceString.Remove(priceString.Length - 4, 3);
        return Int32.Parse(priceString);
    }

}
