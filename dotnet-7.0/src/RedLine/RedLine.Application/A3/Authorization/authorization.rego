package authorization

import data.activities
import data.activityGroups
import data.grants

default authorized = false
default user_has_activity_grant = false
default user_has_resource_grant = false

entrypoint = {
	"isAuthorized": authorized,
    "hasActivityGrant": user_has_activity_grant,
    "hasResourceGrant": user_has_resource_grant
}

authorized {
	user_has_activity_grant
    not activity_has_resource_types
}

authorized {
	user_has_activity_grant
    user_has_resource_grant
}

user_has_activity_grant {
	user_has_grant("Activity", input.activity)
}

user_has_activity_grant {
	activity_in_group[activityGroup]
	user_has_grant("ActivityGroup", activityGroup)
}

user_has_resource_grant {
	resourceType := activities[input.activity].resourceTypes[_]
	user_has_grant(resourceType, input.resource)
}

user_has_resource_grant {
    input.activity == null
	user_has_grant(input.resourceType, input.resource)
}

activity_has_resource_types {
	activities[input.activity].resourceTypes != null
}

activity_in_group[activityGroup] {
    activity = activityGroups[activityGroup].activities[_]
    activity.activityName == input.activity
    activity.tenantId == input.tenantId
}

user_has_grant(t, q) {
	tenantId := ["*", input.tenantId][_] # cycle tenantId possibilities
    type := ["*", t][_] # cycle type possibilities
    qualifier := ["*", q][_]
    grant_is_active(concat(":", [tenantId, type, qualifier]))
}

user_has_grant(t, q) {
	tenantId := ["*", input.tenantId][_] # cycle tenantId possibilities
    qualifier := resource_qualifiers[_]
    grant_is_active(concat(":", [tenantId, t, qualifier]))
}

resource_qualifiers = { q |
	parts = split(input.resource, "/")
    part = parts[i]
    i > 0
    path = concat("/", array.slice(parts, 0, i))
    q := concat("/", [path, "*"])
}

grant_is_active(urn) {
	grant := grants[urn]
    grant.expiresOn == null
}

grant_is_active(urn) {
	grant := grants[urn]
    grant.expiresOn >= input.now
}
