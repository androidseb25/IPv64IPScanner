namespace IPv64IPScanner;

public class PostObject
{
    public string IP { get; set; }
    public int MethodType { get; set; } = (int)BlockMethods.HTTP_S;
}