namespace McCreate.App.Services;

public class MccreateService
{

    private readonly TestService TestService;
    
    public MccreateService(TestService testService)
    {
        TestService = testService;
    }
}