using System;
using System.Collections.Generic;

[Serializable]
public enum JoinType
{
    Instant,
    Request
}

[Serializable]
public class Party
{
    public string _title;
    public string _manager;
    public List<string> _member;
    public JoinType _joinType;
    public int acceptMember;

    public Party(string title, string name, JoinType joinType)
    {
        _title = title;
        _manager = name;
        _member = new List<string> { _manager };
        _joinType = joinType;
        acceptMember = 0;
    }

    public bool CanParticipation(string name)
    {
        if (_member.Contains(name)) return false;
        return _member.Count < 4;
    }

    public bool IsMyParty(string name)
    {
        return _member.Contains(name);
    }
}