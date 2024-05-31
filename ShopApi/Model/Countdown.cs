using Microsoft.EntityFrameworkCore;

namespace ShopApi.Model;


public class Countdown
{
    
    public int Days { get; set; }
    public int hours { get; set; }
    public int min { get; set; }
    public int sec { get; set; }
}