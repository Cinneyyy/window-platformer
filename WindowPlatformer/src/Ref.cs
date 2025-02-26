namespace src;

public class Ref<T>(T value) where T : struct
{
    public T value = value;


    public void Set(T value) => this.value = value;
    public T Get() => value;
}