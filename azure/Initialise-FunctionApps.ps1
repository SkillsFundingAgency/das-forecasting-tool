param(
    [string[]]$FunctionAppNames,
    [string]$ResourceGroupName
)

$SubscriptionId = (Get-AzContext).Subscription.Id
if(!$SubscriptionId){
    $SubscriptionId = (Get-AzContext).Subscription.SubscriptionId
}
foreach ($FunctionApp in $FunctionAppNames) {
    Write-Host "Initialising $FunctionApp"
    $ResourceActionParams = @{
        Action     = "listsecrets"
        ResourceId = "/subscriptions/$SubscriptionId/resourceGroups/$ResourceGroupName/providers/Microsoft.Web/sites/$FunctionApp/functions/InitialiseFunction"
        ApiVersion = "2016-08-01"
        Force      = $true
    }
    $TriggerUrl = (Invoke-AzResourceAction @ResourceActionParams).trigger_url
    Invoke-RestMethod -Uri $TriggerUrl -UseBasicParsing
}
