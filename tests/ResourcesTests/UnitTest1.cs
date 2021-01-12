using NUnit.Framework;
using Resources;

namespace ResourcesTests
{
   public class Tests
   {
      [SetUp]
      public void Setup()
      {
      }

      [Test]
      public void Test1()
      {

         using var reader = new ResourceTextReader(@"D:\Projects\NetCore\WindowsUtilities\tests\ResourcesTests\Resources.resx");

         foreach (var item in reader)
         {

         }


         Assert.Pass();
      }
   }
}