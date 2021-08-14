using Microsoft.EntityFrameworkCore;

namespace Contoso.Unicorn.Entities
{
    public partial class UnicornContext
    {
        /// <inheritdoc />
        public UnicornContext()
        { }

        /// <inheritdoc />
        public UnicornContext(DbContextOptions<UnicornContext> options)
            : base(options)
        { }
    }
}
