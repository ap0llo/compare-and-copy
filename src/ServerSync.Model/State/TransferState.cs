using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Model.State
{
    public class TransferState
    {

        #region Fields

        HashSet<string> m_TransferLocations;

        #endregion

        #region Properties

        public TransferDirection Direction { get; set; }

        public IEnumerable<string> Locations
        {
            get { return m_TransferLocations.ToList(); }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.m_TransferLocations = new HashSet<string>(value, StringComparer.InvariantCultureIgnoreCase);
            }
        }

        #endregion

        #region Constructor

        public TransferState() : this(TransferDirection.None)
        {

        }


        public TransferState(TransferDirection direction) : this(direction, Enumerable.Empty<string>())
        {

        }

        public TransferState(TransferDirection direction, IEnumerable<string> transferLocations)
        {
            if(transferLocations == null)
            {
                throw new ArgumentNullException("transferLocations");
            }

            this.Direction = direction;
            this.m_TransferLocations = new HashSet<string>(transferLocations, StringComparer.InvariantCultureIgnoreCase);
        }

        #endregion

        #region Public Methods

        public void AddTransferLocation(string path)
        {
            m_TransferLocations.Add(path.Trim());            
        }

        public void RemoveTransferLocation(string path)
        {
            m_TransferLocations.Remove(path.Trim());
        }

        #endregion
    }
}
