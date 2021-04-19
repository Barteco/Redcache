using ProtoBuf;
using System;

namespace Redcache.Tests.Fakes
{
    [ProtoContract]
    class Person
    {
        [ProtoMember(1)]
        public string Name { get; set; }
        [ProtoMember(2)]
        public string Address { get; set; }
        [ProtoMember(3)]
        public DateTime DateOfBirth { get; set; }
    }
}
