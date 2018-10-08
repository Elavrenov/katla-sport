import { Component, OnInit } from '@angular/core';
import { HiveSection } from '../models/hive-section';
import { ActivatedRoute, Router } from '@angular/router';
import { HiveSectionService } from '../services/hive-section.service';

@Component({
  selector: 'app-hive-section-form',
  templateUrl: './hive-section-form.component.html',
  styleUrls: ['./hive-section-form.component.css']
})
export class HiveSectionFormComponent implements OnInit {

  hiveSection = new HiveSection(0, "", "", false, "", 0);
  existed = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private hiveSectionService: HiveSectionService
  ) { }

  ngOnInit() {
    this.route.params.subscribe(r => {
      if (r['id'] === undefined){
        this.hiveSection.storeHiveId = r['hiveId'];
        return;
      }
      this.hiveSectionService.getHiveSection(r['id']).subscribe(s => this.hiveSection = s);
      this.existed = true;
    })
  }

  navigateToHiveSections() {
    this.router.navigate([`/hive/${this.hiveSection.storeHiveId}/sections`]);
  }

  onCancel(){
    this.navigateToHiveSections();
  }

  onSubmit() {
    if (this.existed) {
      this.hiveSectionService.updateHiveSection(this.hiveSection).subscribe(c => this.navigateToHiveSections());
    } else {
      this.hiveSectionService.addHiveSection(this.hiveSection).subscribe(c => this.navigateToHiveSections());
    }
  }

   onDelete() {
    this.hiveSectionService.setHiveSectionStatus(this.hiveSection.id, true).subscribe(c => this.hiveSection.isDeleted = true);
  }

   onUndelete() {
    this.hiveSectionService.setHiveSectionStatus(this.hiveSection.id, false).subscribe(c => this.hiveSection.isDeleted = false);
  }

   onPurge() {
    this.hiveSectionService.deleteHiveSection(this.hiveSection).subscribe(c => this.navigateToHiveSections());
  }
}