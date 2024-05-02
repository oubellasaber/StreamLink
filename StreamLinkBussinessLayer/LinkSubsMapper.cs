using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamLinkBussinessLayer
{
    public class LinkSubsMapper
    {
        private enum enHostState { Creating, Editing }
        public int LinkSubsMapperId { get; private set; }
        public int EpLinkId { get; set; }
        public int SubsId { get; set; }
        
        private static List<LinkSubsMapper> ConvertToLinkSubsMappersList(List<object> anonymousLinkSubsMappers)
        {
            List<LinkSubsMapper> linkSubsMappers = new List<LinkSubsMapper>();

            foreach (var anonymousLinkSubsMapper in anonymousLinkSubsMappers)
            {
                linkSubsMappers.Add(new Link(
                    (int)anonymousLinkSubsMapper.GetType().GetProperty("LinkSubsMapperId").GetValue(anonymousLinkSubsMapper),
                    (int)anonymousLinkSubsMapper.GetType().GetProperty("EpLinkId").GetValue(anonymousLinkSubsMapper),
                    (int)anonymousLinkSubsMapper.GetType().GetProperty("SubsId").GetValue(anonymousLinkSubsMapper),
                    ));
            }

            return linkSubsMappers;
        }

        public static List<Link> GetAll(int epLinkId)
        {
            return ConvertToLinkSubsMappersList(StreamLinkDataAccessLayer.LinkSubsMapperDA.GetAll(epLinkId));
        }
    }
